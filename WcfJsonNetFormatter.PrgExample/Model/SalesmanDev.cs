using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentLayer.Domain;

namespace WcfJsonService.Example.Model
{
    public class SalesmanDev
        : Salesman
    {
        public SalesmanDev()
        {
            
        }

        public SalesmanDev(long? id)
            :base(id)
        {

        }

        public string LocalKey { get; set; }
    }
}
