using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using Newtonsoft.Json;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Formatters;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    public class WebHttpJsonNetBehavior
        : WebHttpJsonBehavior
    {
        //private readonly ServiceTypeRegister configRegister;
        private readonly JsonSerializer serializer;

        /// <summary>
        /// 
        /// </summary>
        public WebHttpJsonNetBehavior()
            : this(new List<Type>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="knownTypes"></param>
        public WebHttpJsonNetBehavior(IEnumerable<Type> knownTypes)
        {
            
            SerializerSettings serializerInfo = this.ConfigRegister.SerializerConfig;

            CustomContractResolver resolver = new CustomContractResolver(true, false, this.ConfigRegister.TryToNormalize)
            {
                DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            };

            serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = resolver
            };

            if (!serializerInfo.OnlyPublicConstructor)
                serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            if (serializerInfo.EnablePolymorphicMembers)
            {
                serializer.Binder = new OperationTypeBinder(this.ConfigRegister);
                serializer.TypeNameHandling = TypeNameHandling.Objects;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription,
                                                                                   ServiceEndpoint endpoint)
        {
            return new DispatchJsonNetMessageFormatter(operationDescription, this.serializer, this.ConfigRegister);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription,
                                                                               ServiceEndpoint endpoint)
        {
            return new ClientJsonNetMessageFormatter(operationDescription, endpoint, this.serializer, this.ConfigRegister);
        }

    }
}
