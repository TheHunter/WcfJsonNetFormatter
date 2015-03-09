using System;
using Newtonsoft.Json;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// Class JsonReaderConverter.
    /// </summary>
    public class JsonReaderConverter
        : JsonConverter
    {
        private readonly Type typeConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReaderConverter"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">The object type converter cannot be null.</exception>
        public JsonReaderConverter(Type type)
        {
            if (type == null)
                throw new JsonSerializationException("The object type converter cannot be null.");

            this.typeConverter = type;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">No object created.</exception>
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
        /// Makes the instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException"></exception>
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
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="System.NotSupportedException">JsonReaderConverter should only be used while deserializing.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("JsonReaderConverter should only be used while deserializing.");
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.</value>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
