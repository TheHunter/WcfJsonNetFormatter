using System;
using Newtonsoft.Json;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonReaderConverter
        : JsonConverter
    {
        private readonly Type typeConverter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public JsonReaderConverter(Type type)
        {
            if (type == null)
                throw new JsonSerializationException("The object type converter cannot be null.");

            this.typeConverter = type;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <exception cref="JsonSerializationException"></exception>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            object value = MakeInstance();
            if (value == null)
                throw new JsonSerializationException("No object created.");

            serializer.Populate(reader, value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="JsonSerializationException"></exception>
        /// <returns></returns>
        private object MakeInstance()
        {
            try
            {
                return Activator.CreateInstance(typeConverter, true);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(string.Format("Error on making object by the given type object, type: {0}", typeConverter.FullName), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <exception cref="NotSupportedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("JsonReaderConverter should only be used while deserializing.");
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
