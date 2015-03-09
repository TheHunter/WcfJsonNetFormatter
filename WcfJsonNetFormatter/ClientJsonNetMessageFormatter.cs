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
    /// Class ClientJsonNetMessageFormatter.
    /// </summary>
    public class ClientJsonNetMessageFormatter
        : ClientJsonMessageFormatter
    {
        private readonly Uri operationUri;
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientJsonNetMessageFormatter"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serviceRegister">The service register.</param>
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
        /// Encodes the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Byte[].</returns>
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
        /// Decodes the reply.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Object.</returns>
        public override object DecodeReply(byte[] body, object[] parameters)
        {
            using (MemoryStream ms = new MemoryStream(body))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JToken token = serializer.Deserialize<JToken>(reader);
                        Type type = this.ServiceRegister.GetTypeByName(JsonFormatterUtility.GetTypeNameFromJObject(token as JObject), false)
                                    ?? this.OperationResult.NormalizedType;

                        object ret = token.ToObject(type, serializer);
                        return ret;
                    }
                }
            }
        }
    }
}
