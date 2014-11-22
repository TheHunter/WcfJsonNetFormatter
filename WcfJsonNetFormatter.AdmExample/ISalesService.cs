using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using PersistentLayer.Domain;

namespace WcfJsonService.Example
{
    [ServiceContract(Namespace = "WcfJsonService.Example")]
    [ServiceKnownType("GetKnownTypes", typeof(WcfServiceHolder))]
    public interface ISalesService
    {
        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        Agency GetAgency(long id);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        Agency GetFirstAgency(int pageIndex, int pageSize);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        Salesman GetSalesman(long id);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        Salesman GetFirstSalesman(int pageIndex, int pageSize);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        Salesman UpdateCode(Salesman instance, int code);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        bool VerifyContracts(IEnumerable<TradeContract> contracts);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        void SaveCode(TradeContract contract, long number);

        //[OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        TradeContract GetContract(int id);


        [OperationContract]
        [WebGet(UriTemplate = "/InsertData3?param1={param1}&str={str}")]
        InputData saveDataGet3(InputData param1, string str);
    }

}
