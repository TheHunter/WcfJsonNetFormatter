using System;
using System.IO;
using System.ServiceModel.Description;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WcfJsonFormatter.Formatters;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientJsonNetMessageFormatter
        : ClientJsonMessageFormatter
    {
        private readonly Uri operationUri;
        private readonly JsonSerializer serializer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="endpoint"></param>
        /// <param name="serializer"></param>
        /// <param name="serviceRegister"></param>
        public ClientJsonNetMessageFormatter(OperationDescription operation, ServiceEndpoint endpoint, JsonSerializer serializer, IServiceRegister serviceRegister)
            : base(operation, endpoint, serviceRegister)
        {
            string endpointAddress = endpoint.Address.Uri.ToString();
            if (!endpointAddress.EndsWith("/"))
                endpointAddress = endpointAddress + "/";

            this.operationUri = new Uri(endpointAddress + operation.Name);
            this.serializer = serializer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override byte[] EncodeParameters(object[] parameters)
        {
            byte[] body;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        int index = -1;
                        writer.WriteStartObject();
                        foreach (var parameter in this.OperationParameters)
                        {
                            object paramValue = parameters[++index];
                            writer.WritePropertyName(parameter.Name);

                            if (paramValue == null)
                            {
                                serializer.Serialize(writer, null);
                            }
                            else
                            {
                                JToken current = JToken.FromObject(paramValue, serializer);
                                JsonFormatterUtility.JTokenToSerialize(current);
                                serializer.Serialize(writer, current);
                            }
                        }
                        writer.WriteEndObject();
                        writer.Flush();
                    }
                }
                body = ms.ToArray();
            }
            return body;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override object DecodeReply(byte[] body, object[] parameters)
        {
            using (MemoryStream ms = new MemoryStream(body))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JToken token = serializer.Deserialize<JToken>(reader);
                        Type type = this.ServiceRegister.GetTypeByName(JsonFormatterUtility.JTokenToDeserialize(token), false)
                                    ?? this.OperationResult.NormalizedType;

                        object ret = token.ToObject(type, serializer);
                        return ret;
                    }
                }
            }
        }
    }
}
