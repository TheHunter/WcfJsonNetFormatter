using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Newtonsoft.Json;
using WcfJsonFormatter;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Ns;

namespace WcfJsonService.Example.Extra
{
    public class NewHttpJsonNetBehavior
        : IEndpointBehavior
    {
        public NewHttpJsonNetBehavior(IEnumerable<Type> knownTypes = null)
        {
            var configRegister = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister
                            ?? new ServiceTypeRegister();

            if (knownTypes != null)
                configRegister.LoadTypes(knownTypes);

            this.ConfigRegister = configRegister;

            SerializerSettings serializerInfo = this.ConfigRegister.SerializerConfig;

            CustomContractResolver resolver = new CustomContractResolver(true, false, this.ConfigRegister.TryToNormalize)
            {
                DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            };

            this.Serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = resolver
            };

            if (!serializerInfo.OnlyPublicConstructor)
                Serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            if (serializerInfo.EnablePolymorphicMembers)
            {
                Serializer.Binder = new OperationTypeBinder(this.ConfigRegister);
                Serializer.TypeNameHandling = TypeNameHandling.Objects;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IServiceRegister ConfigRegister { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public JsonSerializer Serializer { get; private set; }


        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            //throw new NotImplementedException();
        }


        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "The endpoint for the given dispatcher cannot be null.");
            if (endpointDispatcher == null)
                throw new ArgumentNullException("endpointDispatcher", "The endpointDispatcher for the given endpoint cannot be null.");

            endpointDispatcher.AddressFilter = new PrefixEndpointAddressMessageFilter(endpoint.Address);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();
            //endpointDispatcher.DispatchRuntime.OperationSelector = (IDispatchOperationSelector)this.GetOperationSelector(endpoint);

            string str = null;
            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                if (operationDescription.Messages[0].Direction == MessageDirection.Input && operationDescription.Messages[0].Action == "*")
                {
                    str = operationDescription.Name;
                    break;
                }
            }

            if (str != null)
                endpointDispatcher.DispatchRuntime.Operations.Add(endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation);
            
            endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation = new DispatchOperation(endpointDispatcher.DispatchRuntime, "*", "*", "*")
            {
                DeserializeRequest = false,
                SerializeReply = false
            };

            //endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Invoker = (IOperationInvoker)new HttpUnhandledOperationInvoker();
            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                DispatchOperation dispatchOperation = null;
                if (endpointDispatcher.DispatchRuntime.Operations.Contains(operationDescription.Name))
                    dispatchOperation = endpointDispatcher.DispatchRuntime.Operations[operationDescription.Name];
                else if (endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Name == operationDescription.Name)
                    dispatchOperation = endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation;
                if (dispatchOperation != null)
                {
                    /*
                    IDispatchMessageFormatter dispatchFormatter1 = this.GetRequestDispatchFormatter(operationDescription, endpoint);
                    IDispatchMessageFormatter dispatchFormatter2 = this.GetReplyDispatchFormatter(operationDescription, endpoint);
                    dispatchOperation.Formatter = (IDispatchMessageFormatter)new CompositeDispatchFormatter(dispatchFormatter1, dispatchFormatter2);
                    dispatchOperation.DeserializeRequest = dispatchFormatter1 != null;
                    dispatchOperation.SerializeReply = operationDescription.Messages.Count > 1 && dispatchFormatter2 != null;
                    */

                    dispatchOperation.Formatter = this.GetDispatchMessageFormatter(operationDescription, endpoint);
                    //dispatchOperation.DeserializeRequest = dispatchFormatter1 != null;
                    dispatchOperation.DeserializeRequest = true;    //formatter.DeserializeRequest(...) is implemented.
                    //dispatchOperation.SerializeReply = operationDescription.Messages.Count > 1 && dispatchFormatter2 != null;
                    dispatchOperation.SerializeReply = operationDescription.Messages.Count > 1; //&& formatter.SerializeReply(...) is implemented.
                }
            }
            this.AddServerErrorHandlers(endpoint, endpointDispatcher);
        }


        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "The endpoint for the given clientRuntime cannot be null.");
            if (clientRuntime == null)
                throw new ArgumentNullException("clientRuntime", "The clientRuntime for the given endpoint cannot be null.");
            
            foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
            {
                if (clientRuntime.Operations.Contains(operationDescription.Name))
                {
                    ClientOperation clientOperation = clientRuntime.Operations[operationDescription.Name];
                    //IClientMessageFormatter requestClientFormatter = this.GetRequestClientFormatter(operationDescription, endpoint);
                    //IClientMessageFormatter replyClientFormatter = this.GetReplyClientFormatter(operationDescription, endpoint);
                    //clientOperation.Formatter = (IClientMessageFormatter)new CompositeClientFormatter(requestClientFormatter, replyClientFormatter);
                    clientOperation.Formatter = this.GetClientMessageFormatter(operationDescription, endpoint);
                    clientOperation.SerializeRequest = true;
                    clientOperation.DeserializeReply = operationDescription.Messages.Count > 1 && !IsUntypedMessage(operationDescription.Messages[1]);
                }
            }
            this.AddClientErrorInspector(endpoint, clientRuntime);
        }


        protected virtual void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(new GlobalErrorHandler(endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults));
        }

        protected virtual void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new WebFaultClientMessageInspector());
        }


        public virtual IDispatchMessageFormatter GetDispatchMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return new DispatchJsonNetMessageFormatter(operationDescription, this.Serializer, this.ConfigRegister);
        }


        public virtual IClientMessageFormatter GetClientMessageFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return new ClientJsonNetMessageFormatter(operationDescription, endpoint, this.Serializer, this.ConfigRegister);
        }

        internal static bool IsUntypedMessage(MessageDescription message)
        {
            if (message == null)
                return false;
            if (message.Body.ReturnValue != null && message.Body.Parts.Count == 0 && message.Body.ReturnValue.Type == typeof(Message))
                return true;
            if (message.Body.ReturnValue == null && message.Body.Parts.Count == 1)
                return message.Body.Parts[0].Type == typeof(Message);
            
            return false;
        }


        public void Validate(ServiceEndpoint endpoint)
        {
            //throw new NotImplementedException();
        }
    }
}
