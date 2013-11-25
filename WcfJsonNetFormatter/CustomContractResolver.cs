using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    internal class CustomContractResolver
        : DefaultContractResolver
    {
        private readonly bool includeFields;
        private readonly Func<Type, Type> normalizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareCache"></param>
        /// <param name="includeFields"></param>
        /// <param name="normalizer"></param>
        public CustomContractResolver(bool shareCache, bool includeFields, Func<Type, Type> normalizer)
            : base(shareCache)
        {
            this.includeFields = includeFields;
            this.normalizer = normalizer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetPropertyMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            return type.GetProperties(flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> properties = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));
            if (!includeFields)
            {
                IEnumerable<string> propertyMembers = CustomContractResolver.GetPropertyMembers(type).Select(n => n.Name);
                properties.RemoveAll(n => !propertyMembers.Contains(n.PropertyName));

                foreach (var property in properties)
                {
                    Type normalized = normalizer.Invoke(property.PropertyType);
                    if (normalized != null && normalized != property.PropertyType)
                        property.MemberConverter = new JsonReaderConverter(normalized);
                }
            }

            return properties;
        }
    }
}
