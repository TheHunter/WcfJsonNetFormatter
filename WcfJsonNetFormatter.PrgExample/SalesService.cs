using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Linq;
using PersistentLayer.Domain;
using WcfExtensions;

namespace WcfJsonService.Example
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SalesService
        : ISalesService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependency1"></param>
        /// <param name="dependency2"></param>
        public SalesService(string dependency1, int dependency2)
        {
            // dependencies for test..
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Agency GetAgency(long id)
        {
            return new Agency { ID = id, Name = "Herbal Company srl", IDManager = 1 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Agency GetFirstAgency(int pageIndex, int pageSize)
        {
            return new Agency {IDManager = 1, Name = "Arnold srl", ID = 1};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Salesman GetSalesman(long id)
        {
            var instance = new Salesman(false) { Name = "Craig", Surname = "Davis", Email = "davis.free@gmail.com", IdentityCode = 255};
            
            instance.GetType().GetProperty("ID")
                    .SetValue(instance, id, null);

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Salesman GetFirstSalesman(int pageIndex, int pageSize)
        {
            var instance = new Salesman(false) { Name = "ciccio", Surname = "pasticcio", Email = "ciccio.pasticcio@gmail.com", IdentityCode = 255 };

            instance.GetType().GetProperty("ID")
                    .SetValue(instance, 100L, null);

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize)
        {
            List<Salesman> l1 = new List<Salesman>();
            l1.Add(new Salesman(false) { Name = "Craig", Surname = "Davis", Email = "davis.free@gmail.com", IdentityCode = 255});
            l1.Add(new Salesman(false) { Name = "ciccio", Surname = "pasticcio", Email = "ciccio.pasticcio@gmail.com", IdentityCode = 255 });
            return l1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize)
        {
            var sal1 = new Salesman(false) { Name = "Craig", Surname = "Davis", Email = "davis.free@gmail.com", IdentityCode = 255 };

            sal1.GetType().GetProperty("ID")
                    .SetValue(sal1, 100L, null);

            TradeContract instance1 = new CarContract
            {
                BeginDate = new DateTime(2010, 10, 1),
                BrandName = "Porsche",
                Description = "fattura di test",
                Number = 12000000,
                Owner = sal1,
                Price = 50500
            };

            
            var sal2 = new Salesman(false) { Name = "james", Surname = "hold", Email = "james.hold@gmail.com", IdentityCode = 255 };
            sal2.GetType().GetProperty("ID")
                    .SetValue(sal1, 115L, null);

            TradeContract instance2 = new CarContract
            {
                BeginDate = new DateTime(2011, 10, 1),
                BrandName = "Porsche",
                Description = "fattura di test",
                Number = 12000000,
                Owner = sal2,
                Price = 50500
            };

            return new List<TradeContract>{instance1, instance2};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Salesman UpdateCode(Salesman instance, int code)
        {
            if (instance == null)
                return null;

            instance.IdentityCode = code;
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public bool VerifyContracts(IEnumerable<TradeContract> contracts)
        {
            return (contracts != null && contracts.Any());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="number"></param>
        public void SaveCode(TradeContract contract, long number)
        {
            if (contract != null)
                contract.Number = number;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TradeContract GetContract(int id)
        {
            var sal = new Salesman(false) { Name = "Craig", Surname = "Davis", Email = "davis.free@gmail.com", IdentityCode = 255 };

            sal.GetType().GetProperty("ID")
                    .SetValue(sal, Convert.ToInt64(id), null);

            TradeContract instance = new CarContract
                {
                    BeginDate = new DateTime(2010, 10, 1),
                    BrandName = "Porsche",
                    Description = "fattura di test",
                    Number = 12000000,
                    Owner = sal,
                    Price = 50500
                };
            return instance;
        }
    }
}
