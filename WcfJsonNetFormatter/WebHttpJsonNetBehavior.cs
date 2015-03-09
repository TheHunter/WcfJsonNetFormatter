using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using Newtonsoft.Json;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Formatters;
using System.ServiceModel.Dispatcher;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// Class WebHttpJsonNetBehavior.
    /// </summary>
    public class WebHttpJsonNetBehavior
        : WebHttpJsonBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebHttpJsonNetBehavior"/> class.
        /// </summary>
        public WebHttpJsonNetBehavior()
            : this(new List<Type>(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHttpJsonNetBehavior"/> class.
        /// </summary>
        /// <param name="knownTypes">The known types.</param>
        /// <param name="enableUriTemplate">if set to <c>true</c> [enable URI template].</param>
        public WebHttpJsonNetBehavior(IEnumerable<Type> knownTypes, bool enableUriTemplate = true)
            : base(knownTypes, enableUriTemplate)
        {
            
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
        /// Makes the dispatch message formatter.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>IDispatchJsonMessageFormatter.</returns>
        public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription,
                                                                                   ServiceEndpoint endpoint)
        {
            return new DispatchJsonNetMessageFormatter(operationDescription, this.Serializer, this.ConfigRegister);
        }

        /// <summary>
        /// Makes the client message formatter.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>IClientJsonMessageFormatter.</returns>
        public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription,
                                                                               ServiceEndpoint endpoint)
        {
            return new ClientJsonNetMessageFormatter(operationDescription, endpoint, this.Serializer, this.ConfigRegister);
        }

        /// <summary>
        /// Gets the query string converter.
        /// </summary>
        /// <param name="operationDescription">The service operation.</param>
        /// <returns>A <see cref="T:System.ServiceModel.Dispatcher.QueryStringConverter" /> instance.</returns>
        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return new QueryStringJsonConverter(this.Serializer, this.ConfigRegister);
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <value>The serializer.</value>
        public JsonSerializer Serializer { get; private set; }
        

        
    }
}
