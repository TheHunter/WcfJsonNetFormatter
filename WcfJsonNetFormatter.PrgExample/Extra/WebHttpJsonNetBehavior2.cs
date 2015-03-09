using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Newtonsoft.Json;
using WcfJsonFormatter;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Formatters;
using WcfJsonFormatter.Ns;

namespace WcfJsonService.Example.Extra
{
    public class WebHttpJsonNetBehavior2
        : WebHttpJsonBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        public WebHttpJsonNetBehavior2()
            : this(new List<Type>(), true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="knownTypes"></param>
        /// <param name="enableUriTemplate"></param>
        public WebHttpJsonNetBehavior2(IEnumerable<Type> knownTypes, bool enableUriTemplate = true)
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

        /// <inheritdoc/>
        public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription,
                                                                                   ServiceEndpoint endpoint)
        {
            return new DispatchJsonNetMessageFormatter(operationDescription, this.Serializer, this.ConfigRegister);
        }

        /// <inheritdoc/>
        public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription,
                                                                               ServiceEndpoint endpoint)
        {
            return new ClientJsonNetMessageFormatter(operationDescription, endpoint, this.Serializer, this.ConfigRegister);
        }

        /// <inheritdoc/>
        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return new QueryStringJsonConverter(this.Serializer, this.ConfigRegister);
        }

        /// <summary>
        /// 
        /// </summary>
        public JsonSerializer Serializer { get; private set; }
    }
}
