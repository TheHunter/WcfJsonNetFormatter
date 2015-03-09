using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// Class JsonFormatterUtility.
    /// </summary>
    public static class JsonFormatterUtility
    {
        /// <summary>
        /// the token to serialize.
        /// </summary>
        /// <param name="token">The token.</param>
        internal static void JTokenToSerialize(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    {
                        JTokenToSerialize(token as JArray);
                        break;
                    }
                case JTokenType.Property:
                    {
                        JTokenToSerialize(token as JProperty);
                        break;
                    }
                case JTokenType.Object:
                    {
                        JTokenToSerialize(token as JObject);
                        break;
                    }
            }
        }

        /// <summary>
        /// the token to serialize.
        /// </summary>
        /// <param name="token">The token.</param>
        internal static void JTokenToSerialize(JProperty token)
        {
            if (token.Name == "$type")
            {
                string nameType = token.Value.ToString().Trim();
                if (!nameType.Equals(string.Empty))
                {
                    token.Value = NormalizeTypeName(token.Value.ToString());
                }
            }
            else
            {
                JTokenToSerialize(token.Value);
            }
        }

        /// <summary>
        /// the token to serialize.
        /// </summary>
        /// <param name="token">The token.</param>
        internal static void JTokenToSerialize(JArray token)
        {
            foreach (var current in token)
            {
                JTokenToSerialize(current);
            }
        }

        /// <summary>
        /// the token to serialize.
        /// </summary>
        /// <param name="token">The token.</param>
        internal static void JTokenToSerialize(JObject token)
        {
            if (token != null)
            {
                foreach (var property in token.Properties())
                {
                    JTokenToSerialize(property);
                }

                var jType = token.Property("$type");
                if (jType != null)
                {
                    var val = jType.Value.ToString().Trim();
                    if (val.Equals(string.Empty))
                        jType.Remove();
                }
            }
        }

        /// <summary>
        /// Normalizes the name of the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>System.String.</returns>
        internal static string NormalizeTypeName(string typeName)
        {
            if (typeName == null)
                return null;

            typeName = typeName.Trim();

            int index = typeName.IndexOf(',');
            if (index > -1)
                typeName = typeName.Substring(0, index).Trim();

            return typeName.Substring(typeName.LastIndexOf('.') + 1).Trim();
        }

        /// <summary>
        /// Gets the type name from jobject.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.String.</returns>
        internal static string GetTypeNameFromJObject(JObject token)
        {
            if (token == null)
                return null;

            JProperty typeProperty = token.Property("$type");
            if (typeProperty == null)
                return null;

            string typeName = typeProperty.Value.ToString();
            token.Remove(typeProperty.Name);
            return typeName;
        }

        /// <summary>
        /// Determines whether [is jarray type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is j array type] [the specified type]; otherwise, <c>false</c>.</returns>
        internal static bool IsJArrayType(Type type)
        {
            if (type == null)
                return false;

            if (type.IsArray)
                return true;

            Type collectionType = type.GetInterface("IEnumerable", true)
                                  ?? type.GetInterface("IEnumerable`1", true);

            return collectionType != null;
        }

        /// <summary>
        /// Encodes the given string in base64.
        /// </summary>
        /// <param name="toEncode">To encode.</param>
        /// <returns>System.String.</returns>
        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.Encoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Decodes the string from base64.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <returns>System.String.</returns>
        public static string DecodeFrom64(this string encodedData)
        {
            byte[] encodedDataAsBytes
                = Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Determines whether [is base64 string] [the specified string].
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns><c>true</c> if [is base64 string] [the specified string]; otherwise, <c>false</c>.</returns>
        public static bool IsBase64String(this string str)
        {
            str = str.Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// Makes the settings.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <returns>JsonSerializerSettings.</returns>
        public static JsonSerializerSettings MakeSettings(this JsonSerializer serializer)
        {
            if (serializer == null)
                return null;

            return new JsonSerializerSettings
            {
                Binder = serializer.Binder,
                CheckAdditionalContent = serializer.CheckAdditionalContent,
                ConstructorHandling = serializer.ConstructorHandling,
                Context = serializer.Context,
                ContractResolver = serializer.ContractResolver,
                Converters = serializer.Converters,
                Culture = serializer.Culture,
                DateFormatHandling = serializer.DateFormatHandling,
                DateFormatString = serializer.DateFormatString,
                DateParseHandling = serializer.DateParseHandling,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                DefaultValueHandling = serializer.DefaultValueHandling,
                //Error = serializer.Error
                FloatFormatHandling = serializer.FloatFormatHandling,
                FloatParseHandling = serializer.FloatParseHandling,
                Formatting = serializer.Formatting,
                MaxDepth = serializer.MaxDepth,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                PreserveReferencesHandling = serializer.PreserveReferencesHandling,
                ReferenceLoopHandling = serializer.ReferenceLoopHandling,
                ReferenceResolver = serializer.ReferenceResolver,
                StringEscapeHandling = serializer.StringEscapeHandling,
                TraceWriter = serializer.TraceWriter,
                TypeNameAssemblyFormat = serializer.TypeNameAssemblyFormat,
                TypeNameHandling = serializer.TypeNameHandling
            };
        }
    }
}
