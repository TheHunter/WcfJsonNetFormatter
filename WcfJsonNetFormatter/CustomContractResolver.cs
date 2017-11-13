using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// Class CustomContractResolver.
    /// </summary>
    public class CustomContractResolver
        : DefaultContractResolver
    {
        private readonly bool includeFields;
        private readonly Func<Type, Type> normalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContractResolver"/> class.
        /// </summary>
        /// <param name="includeFields">if set to <c>true</c> [include fields].</param>
        /// <param name="normalizer">The normalizer.</param>
        public CustomContractResolver(bool includeFields, Func<Type, Type> normalizer)
        {
            this.includeFields = includeFields;
            this.normalizer = normalizer;
        }
        
        /// <summary>
        /// Gets the property members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> GetPropertyMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            return type.GetProperties(flags);
        }

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
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
