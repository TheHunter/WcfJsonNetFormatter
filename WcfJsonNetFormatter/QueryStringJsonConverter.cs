using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace WcfJsonFormatter.Ns
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryStringJsonConverter
        : QueryStringConverter
    {
        private JsonSerializer serializer;
        private JsonSerializerSettings settings;
        private IServiceRegister serviceRegister;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="serviceRegister"></param>
        public QueryStringJsonConverter(JsonSerializer serializer, IServiceRegister serviceRegister)
        {
            this.serializer = serializer;
            this.serviceRegister = serviceRegister;
            this.settings = serializer.MakeSettings();

        }

        /// <inheritdoc/>
        public override bool CanConvert(Type type)
        {
            return true;
        }

        /// <inheritdoc/>
        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            object ret = null;

            if (parameterType == typeof(string))
                return parameter;

            if (parameterType.IsPrimitive)
            {
                ret = Convert.ChangeType(parameter, parameterType);
            }
            else
            {
                if (parameterType.IsInterface)
                    parameterType = this.serviceRegister.TryToNormalize(parameterType);

                ret = JsonConvert.DeserializeObject(parameter, parameterType, this.settings);
            }

            return ret;
        }

    }
}
