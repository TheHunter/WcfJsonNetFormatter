using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace WcfJsonService.Example
{
    public class InputData
    {
        public string FirstName{get; set;}
        public string LastName{get; set;}
    }

    public enum MyEnum
    {
        first = 1,
        second = 2
    }

    [ServiceContract]
    public interface ITest
    {
        [OperationContract]
        [WebGet(UriTemplate = "/InsertData?param1={param1}")]
        string saveDataGet(InputData param1);
        
        [OperationContract]
        [WebInvoke(UriTemplate = "/InsertData")]
        string saveDataPost(InputData param1);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/InsertData2")]
        string saveDataPost2(InputData param1, string str);

        [OperationContract]
        [WebGet(UriTemplate = "/InsertData2?param1={param1}&str={str}")]
        string saveDataGet2(InputData param1, string str);

        [OperationContract(Name = "InsertData3")]
        //[WebGet(UriTemplate = "/InsertData3?param1={param1}&str={str}")]
        [WebGet]
        InputData saveDataGet3(InputData param1, string str);

        [OperationContract(Name = "InsertData4")]
        [WebGet]
        InputData saveDataGet4(InputData param1, string str);

        [OperationContract(Name = "InsertData5")]
        [WebGet]
        InputData saveDataGet5(InputData param1, int str);

        [OperationContract(Name = "InsertData6")]
        [WebGet]
        InputData saveDataGet6(InputData param1, MyEnum str);
    }

    public class Service : ITest
    {
        public string saveDataGet(InputData param1)
        {
            return "Via GET: " + param1.FirstName + " " + param1.LastName;
        }

        public string saveDataPost(InputData param1)
        {
            return "Via POST: " + param1.FirstName + " " + param1.LastName;
        }

        public string saveDataPost2(InputData param1, string str)
        {
            return "Via POST: " + param1.FirstName + " " + param1.LastName + " - " + str;
        }

        public string saveDataGet2(InputData param1, string str)
        {
            return "Via GET: " + param1.FirstName + " " + param1.LastName + " - " + str;
        }

        public InputData saveDataGet3(InputData param1, string str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData{FirstName = param1.FirstName + "__", LastName = param1.LastName + "__"};
        }

        public InputData saveDataGet4(InputData param1, string str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData { FirstName = param1.FirstName + "__", LastName = param1.LastName + "__" };
        }

        public InputData saveDataGet5(InputData param1, int str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData { FirstName = param1.FirstName + "__", LastName = param1.LastName + "__" };
        }

        public InputData saveDataGet6(InputData param1, MyEnum str)
        {
            if (param1 == null)
                return new InputData();

            return new InputData { FirstName = param1.FirstName + "__", LastName = param1.LastName + "__" };
        }
    }


    [ServiceContract]
    public interface ITestLocal
    {
        [OperationContract(Name = "InsertData3")]
        //[WebGet(UriTemplate = "/InsertData3?param1={param1}&str={str}")]
        [WebGet]
        InputData saveDataGet3(InputData param1, string str);

        [OperationContract(Name = "InsertData4")]
        [WebGet]
        InputData saveDataGet4(InputData param1, string str);

        [OperationContract(Name = "InsertData5")]
        [WebGet]
        InputData saveDataGet5(InputData param1, int str);

        [OperationContract(Name = "InsertData6")]
        [WebGet]
        InputData saveDataGet6(InputData param1, MyEnum str);
    }


    public class ServiceLocal
        : ClientBase<ITestLocal>, ITestLocal
    {
        public ServiceLocal(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }
        public InputData saveDataGet3(InputData param1, string str)
        {
            return this.Channel.saveDataGet3(param1, str);
        }
        public InputData saveDataGet4(InputData param1, string str)
        {
            return this.Channel.saveDataGet4(param1, str);
        }
        public InputData saveDataGet5(InputData param1, int str)
        {
            return this.Channel.saveDataGet5(param1, str);
        }
        public InputData saveDataGet6(InputData param1, MyEnum str)
        {
            return this.Channel.saveDataGet6(param1, str);
        }
    }
}
