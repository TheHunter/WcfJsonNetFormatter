using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using PersistentLayer.Domain;
using WcfJsonFormatter;
using WcfJsonFormatter.Ns;

namespace WcfJsonClient.Example
{
    public class WcfClient
    {
        static void Main()
        {
            WcfClient client = new WcfClient();
            //client.Run();
            client.RunJsonProxy();
            //client.RunJsonProxy2();
        }


        private void Run()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service.svc";

            //WebHttpBinding webBinding = new WebHttpBinding { ContentTypeMapper = new RawContentMapper(), MaxReceivedMessageSize = 4194304 };
            //ChannelFactory<ISalesService> channelFactory = new ChannelFactory<ISalesService>(webBinding, new EndpointAddress(baseAddress + "/json"));
            //channelFactory.Endpoint.Behaviors.Add(new WebHttpJsonBehavior());

            ChannelFactory<ISalesService> channelFactory = new ChannelFactory<ISalesService>(new BasicHttpBinding(), new EndpointAddress(baseAddress));

            ISalesService client = channelFactory.CreateChannel();

            //var sales = client.GetPagedSalesman(0, 10);
            //Console.WriteLine(string.Format("Numero di venditori: {0}", sales.Count()));
            //Salesman sal = client.GetPagedSalesman(1);
            //Console.WriteLine(sal);

            //var cc = client.GetFirstAgency(0, 10);
            //Console.WriteLine(cc);
            //Console.WriteLine();

            var cs = client.GetSalesman(1);
            Console.WriteLine(cs);
            Console.WriteLine();

            var cons = client.GetFirstSalesman(0, 10);
            Console.WriteLine(cons);
            Console.WriteLine();

            var ag = client.GetAgency(1);
            Console.WriteLine(ag);
            Console.WriteLine();

            ((System.ServiceModel.Channels.IChannel)client).Close();
            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }


        private void RunJsonProxy()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service.svc";

            WebHttpBinding webBinding = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304,
                SendTimeout = TimeSpan.FromMinutes(4)
            };
            
            EndpointAddress endpoint = new EndpointAddress(baseAddress + "/json");

            using (SalesService proxy = new SalesService(webBinding, endpoint))
            {
                proxy.Endpoint.Behaviors.Add(new WebHttpJsonNetBehavior());
                
                try
                {
                    Agency ag = proxy.GetAgency(1);
                    Console.WriteLine(ag);
                    Console.WriteLine();

                    Salesman cons = proxy.GetSalesman(1);
                    Console.WriteLine(cons);
                    Console.WriteLine();

                    Salesman res = proxy.GetFirstSalesman(0, 10);
                    Console.WriteLine(res);
                    Console.WriteLine();

                    IEnumerable<Salesman> conss = proxy.GetPagedSalesman(0, 2);
                    Console.WriteLine(conss);
                    Console.WriteLine();

                    IEnumerable<TradeContract> ctr = proxy.GetPagedContract(0, 10);
                    Console.WriteLine(ctr);
                    Console.WriteLine();

                    var sal = proxy.UpdateCode(new Salesman(true) {Name = "Manuel", Surname = "Lara"}, 150);
                    Console.WriteLine(sal);
                    Console.WriteLine();

                    var sal2 = proxy.UpdateCode(null, 200);
                    Console.WriteLine("sal2 value:{0}", sal2);
                    Console.WriteLine();

                    IList<TradeContract> ctrs = new List<TradeContract>();
                    ctrs.Add(new CarContract{ Price = 10000, Description = "price car"});
                    ctrs.Add(new HomeContract{ Price = 250000, Description = "price home", Town = "sex city"});
                    var col = proxy.VerifyContracts(ctrs);
                    Console.WriteLine(col);
                    Console.WriteLine();

                    var col2 = proxy.VerifyContracts(null);
                    Console.WriteLine("col2 value: {0}", col2);
                    Console.WriteLine();

                    proxy.SaveCode(new CarContract{ Description = "my car", Price = 25000}, 200);
                    Console.WriteLine("TradeContract saved");
                    Console.WriteLine();

                    TradeContract contract = proxy.GetContract(1);
                    Console.WriteLine("TradeContract value: {0}", contract);
                    Console.WriteLine();

                    Console.WriteLine("Press <ENTER> to terminate client.");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex.Message);

                    Console.WriteLine("Inner message:");
                    Console.WriteLine(ex.InnerException == null ? string.Empty : ex.InnerException.Message);
                    Console.WriteLine();

                    Console.ReadLine();
                }
            }
        }

    }
}
