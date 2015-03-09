using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace WcfJsonService.Example
{
    [ServiceContract]
    public interface ILocalService
    {
        [OperationContract(Name = "readInputData1")]
        [WebGet]
        InputData ReadInputData1(InputData param1, string str);

        [OperationContract(Name = "readInputData2")]
        [WebInvoke]
        InputData ReadInputData2(InputData param1, string str);
    }

    public class LocalService
        : ILocalService
    {
        public InputData ReadInputData1(InputData param1, string str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData { FirstName = param1.FirstName + "__", LastName = param1.LastName + "__" };
        }

        public InputData ReadInputData2(InputData param1, string str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData { FirstName = param1.FirstName + "__", LastName = param1.LastName + "__" };
        }
    }

    public class ProxyService
        : ClientBase<ILocalService>, ILocalService
    {
        public ProxyService(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public InputData ReadInputData1(InputData param1, string str)
        {
            return this.Channel.ReadInputData1(param1, str);
        }

        public InputData ReadInputData2(InputData param1, string str)
        {
            return this.Channel.ReadInputData2(param1, str);
        }
    }
}
