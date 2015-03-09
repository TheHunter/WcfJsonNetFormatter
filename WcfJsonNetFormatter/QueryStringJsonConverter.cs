using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// Class QueryStringJsonConverter.
    /// </summary>
    public class QueryStringJsonConverter
        : QueryStringConverter
    {
        private JsonSerializer serializer;
        private readonly JsonSerializerSettings settings;
        private readonly IServiceRegister serviceRegister;


        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringJsonConverter"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serviceRegister">The service register.</param>
        public QueryStringJsonConverter(JsonSerializer serializer, IServiceRegister serviceRegister)
        {
            this.serializer = serializer;
            this.serviceRegister = serviceRegister;
            this.settings = serializer.MakeSettings();
        }

        /// <summary>
        /// Determines whether the specified type can be converted to and from a string representation.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> to convert.</param>
        /// <returns>A value that specifies whether the type can be converted.</returns>
        public override bool CanConvert(Type type)
        {
            return true;
        }

        /// <summary>
        /// Converts a query string parameter to the specified type.
        /// </summary>
        /// <param name="parameter">The string form of the parameter and value.</param>
        /// <param name="parameterType">The <see cref="T:System.Type" /> to convert the parameter to.</param>
        /// <returns>The converted parameter.</returns>
        /// <exception cref="System.InvalidOperationException">Error when the serializer tried to deserialize the given parameter.</exception>
        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            try
            {
                if (parameterType.IsInterface)
                    parameterType = this.serviceRegister.TryToNormalize(parameterType);

                return JsonConvert.DeserializeObject(parameter, parameterType, this.settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error when the serializer tried to deserialize the given parameter.", ex);
            }
        }

        /// <summary>
        /// Converts a parameter to a query string representation.
        /// </summary>
        /// <param name="parameter">The parameter to convert.</param>
        /// <param name="parameterType">The <see cref="T:System.Type" /> of the parameter to convert.</param>
        /// <returns>The parameter name and value.</returns>
        /// <exception cref="System.InvalidOperationException">Error when the serializer tried to serialize the given parameter.</exception>
        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            try
            {
                return JsonConvert.SerializeObject(parameter, Formatting.None, this.settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error when the serializer tried to serialize the given parameter.", ex);
            }
        }

    }
}
