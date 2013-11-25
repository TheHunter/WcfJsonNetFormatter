using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using PersistentLayer.Domain;

namespace WcfJsonService.Example
{
    /// <summary>
    /// 
    /// </summary>
    public static class WcfServiceHolder
    {
        private static readonly IEnumerable<Type> KnownTypes;

        static WcfServiceHolder()
        {
            KnownTypes = Assembly.GetAssembly(typeof(Salesman)).GetTypes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return KnownTypes;
        }

    }
}
