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
    /// Class DispatchJsonNetMessageFormatter.
    /// </summary>
    public class DispatchJsonNetMessageFormatter
        : DispatchJsonMessageFormatter
    {
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchJsonNetMessageFormatter"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serviceRegister">The service register.</param>
        public DispatchJsonNetMessageFormatter(OperationDescription operation, JsonSerializer serializer, IServiceRegister serviceRegister)
            : base(operation, serviceRegister)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// Decodes the parameters.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="parameters">The parameters.</param>
        public override void DecodeParameters(byte[] body, object[] parameters)
        {
            using (MemoryStream ms = new MemoryStream(body))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JObject wrappedParameters = serializer.Deserialize<JObject>(reader);
                        int indexParam = -1;

                        foreach (var parameter in this.OperationParameters)
                        {
                            JProperty property = wrappedParameters.Property(parameter.Name);
                            if (property != null)
                            {
                                Type type = this.ServiceRegister.GetTypeByName(JsonFormatterUtility.GetTypeNameFromJObject(property.Value as JObject), false)
                                            ?? parameter.NormalizedType;

                                /* $type overrides the given type parameter.
                                 * only if $type is at the first property on the given token..
                                */
                                parameters[++indexParam] = property.Value.ToObject(type, serializer);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Encodes the reply.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="result">The result.</param>
        /// <returns>System.Byte[].</returns>
        public override byte[] EncodeReply(object[] parameters, object result)
        {
            byte[] body;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        if (result == null)
                        {
                            serializer.Serialize(writer, null);
                        }
                        else
                        {
                            JToken token = JToken.FromObject(result, serializer);
                            JsonFormatterUtility.JTokenToSerialize(token);
                            serializer.Serialize(writer, token);
                        }
                        writer.Flush();
                    }
                }
                body = ms.ToArray();
            }
            return body;
        }
    }
}
