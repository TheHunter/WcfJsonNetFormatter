using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
        //[WebGet(UriTemplate = "/InsertData2?param1={param1}&str={str}")]
        //[WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/InsertData2?param1={param1}&str={str}")] //OK..
        //[WebInvoke(UriTemplate = "/InsertData2?param1={param1}&str={str}", Method = "POST")]  //KO... usare WebGet
        //[WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "?param1={param1}&str={str}")]  //tolgo partial URI... KO
        [WebGet(UriTemplate = "/InsertData2?param1={param1}&str={str}")]
        string saveDataGet2(InputData param1, string str);

        [OperationContract]
        [WebGet(UriTemplate = "/InsertData3?param1={param1}&str={str}")]
        InputData saveDataGet3(InputData param1, string str);
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
            return new InputData{FirstName = param1.FirstName + "__", LastName = param1.LastName + "__"};
        }
    }

    
}
