using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using PersistentLayer.Domain;

namespace WcfJsonClient.Example
{
    [ServiceContract]
    public interface ISalesService
    {
        [OperationContract]
        Agency GetAgency(long id);

        [OperationContract]
        Agency GetFirstAgency(int pageIndex, int pageSize);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        Salesman GetSalesman(long id);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        Salesman GetFirstSalesman(int pageIndex, int pageSize);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize);

        [OperationContract]
        IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize);

        [OperationContract]
        Salesman UpdateCode(Salesman instance, int code);

        [OperationContract]
        bool VerifyContracts(IEnumerable<TradeContract> contracts);

        [OperationContract]
        void SaveCode(TradeContract contract, long number);

        [OperationContract]
        TradeContract GetContract(int id);
    }


    public class SalesService
        : ClientBase<ISalesService>, ISalesService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointName"></param>
        public SalesService(string endpointName)
            :base(endpointName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public SalesService(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }


        public Agency GetAgency(long id)
        {
            return this.Channel.GetAgency(id);
        }

        public Agency GetFirstAgency(int pageIndex, int pageSize)
        {
            return this.Channel.GetFirstAgency(pageIndex, pageSize);
        }

        public Salesman GetSalesman(long id)
        {
            return this.Channel.GetSalesman(id);
        }

        public Salesman GetFirstSalesman(int pageIndex, int pageSize)
        {
            return this.Channel.GetFirstSalesman(pageIndex, pageSize);
        }

        public IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize)
        {
            return this.Channel.GetPagedSalesman(pageIndex, pageSize);
        }

        public IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize)
        {
            return this.Channel.GetPagedContract(pageIndex, pageSize);
        }

        public Salesman UpdateCode(Salesman instance, int code)
        {
            return this.Channel.UpdateCode(instance, code);
        }

        public bool VerifyContracts(IEnumerable<TradeContract> contracts)
        {
            return this.Channel.VerifyContracts(contracts);
        }

        public void SaveCode(TradeContract contract, long number)
        {
            this.Channel.SaveCode(contract, number);
        }

        public TradeContract GetContract(int id)
        {
            return Channel.GetContract(id);
        }
    }
}
