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
    public class DispatchJsonNetMessageFormatter
        : DispatchJsonMessageFormatter
    {
        private readonly JsonSerializer serializer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="serializer"></param>
        /// <param name="serviceRegister"></param>
        public DispatchJsonNetMessageFormatter(OperationDescription operation, JsonSerializer serializer, IServiceRegister serviceRegister)
            : base(operation, serviceRegister)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
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
                                Type type = this.ServiceRegister.GetTypeByName(JsonFormatterUtility.JTokenToDeserialize(property.Value), false)
                                            ?? parameter.NormalizedType;

                                //this.ServiceRegister.TryToNormalize();
                                // NOTA: se l'oggetto type non fosse nullo, viene sempre richiamato il binder
                                // in presenza della proprietà $id ??

                                parameters[++indexParam] = property.Value.ToObject(type, serializer);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
