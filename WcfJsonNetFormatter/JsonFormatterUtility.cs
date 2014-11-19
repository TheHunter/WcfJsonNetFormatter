using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonFormatterUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
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
        /// 
        /// </summary>
        /// <param name="token"></param>
        internal static void JTokenToSerialize(JProperty token)
        {
            if (token.Name == "$type")
                token.Value = NormalizeTypeName(token.Value.ToString());
            else
                JTokenToSerialize(token.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        internal static void JTokenToSerialize(JArray token)
        {
            foreach (var current in token)
            {
                JTokenToSerialize(current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        internal static void JTokenToSerialize(JObject token)
        {
            if (token != null)
            {
                foreach (var property in token.Properties())
                {
                    JTokenToSerialize(property);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static string JTokenToDeserialize(JToken token)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static string JTokenToDeserialize(JObject token)
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
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.Encoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        public static string DecodeFrom64(this string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(this string str)
        {
            str = str.Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
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
