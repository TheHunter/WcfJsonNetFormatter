using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Autofac;
using Autofac.Integration.Wcf;
using PersistentLayer.Domain;
using WcfJsonFormatter;
using WcfJsonFormatter.Ns;
using Newtonsoft.Json;
using System.Net;
using WcfJsonService.Example.Extra;

namespace WcfJsonService.Example
{
    public class WcfHost
    {
        static void Main()
        {
            WcfHost host = new WcfHost();
            
            host.Initialize();
            //host.Run();

            host.RunServiceWithWebRequest();
            //host.RunServiceWithProxy();
            //host.RunServiceWithProxy2();
        }

        private void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterInstance("string dependency");
            
            builder.Register(n => 1)
                   .AsSelf();

            builder.RegisterType<SalesService>()
                   .As<ISalesService>();

            AutofacHostFactory.Container = builder.Build();

        }

        private void Run()
        {
            Console.WriteLine("Run a ServiceHost via programmatic configuration...");
            Console.WriteLine();

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service.svc";
            Console.WriteLine("BaseAddress: {0}", baseAddress);


            using (ServiceHost serviceHost = new ServiceHost(typeof(SalesService), new Uri(baseAddress)))
            {
                WebHttpBinding webBinding = new WebHttpBinding
                {
                    ContentTypeMapper = new RawContentMapper(),
                    MaxReceivedMessageSize = 4194304,
                    MaxBufferSize = 4194304
                };

                serviceHost.AddServiceEndpoint(typeof(ISalesService), webBinding, "json")
                    .Behaviors.Add(new WebHttpJsonNetBehavior());

                //serviceHost.AddServiceEndpoint(typeof(ISalesService), new BasicHttpBinding(), baseAddress);
                serviceHost.AddDependencyInjectionBehavior<ISalesService>(AutofacHostFactory.Container);

                Console.WriteLine("Opening the host");
                serviceHost.Open();

                try
                {
                    AutofacHostFactory.Container.Resolve<ISalesService>();
                    Console.WriteLine("The service is ready.");
                    Console.WriteLine("Press <ENTER> to terminate service.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on initializing the service host.");
                    Console.WriteLine(ex.Message);
                }
                
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
        }

        public void RunServiceWithWebRequest()
        {
            WebHttpBinding webBinding = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304
            };

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/jargs";
            Uri uriBase = new Uri(baseAddress);

            ServiceHost host = new ServiceHost(typeof(Service), uriBase);
            host.AddServiceEndpoint(typeof(ITest), webBinding, uriBase)
                            .Behaviors.Add(new WebHttpJsonNetBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = null;
            
            //client = new WebClient();
            //client.Headers[HttpRequestHeader.ContentType] = "application/json";
            //Console.WriteLine(client.UploadString(baseAddress + "/InsertData", "{param1:{\"FirstName\":\"John\",\"LastName\":\"Doe\"}}"));
            
            //client = new WebClient();
            //Console.WriteLine(client.DownloadString(baseAddress + "/InsertData3?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=\"string test\""));

            //Console.WriteLine("new calling...");
            //Console.WriteLine();

            //client = new WebClient();
            //Console.WriteLine(client.DownloadString(baseAddress + "/InsertData4?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=\"string test\""));
            //Console.WriteLine();

            //client = new WebClient();
            //Console.WriteLine(client.DownloadString(baseAddress + "/InsertData5?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=5"));
            //Console.WriteLine();

            //client = new WebClient();
            //Console.WriteLine(client.DownloadString(baseAddress + "/InsertData6?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=1"));
            //Console.WriteLine();

            client = new WebClient();
            Console.WriteLine(client.DownloadString(baseAddress + "/InsertData6?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=\"second\""));
            Console.WriteLine();

            client = new WebClient();
            Console.WriteLine(client.DownloadString(baseAddress + "/InsertData6?param1={\"FirstName\":\"John\",\"LastName\":\"Doe\"}&str=2"));
            Console.WriteLine();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }

        public void RunServiceWithProxy()
        {
            WebHttpBinding webBinding = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304
            };

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/jargs";
            Uri uriBase = new Uri(baseAddress);

            ServiceHost host = new ServiceHost(typeof(Service), uriBase);
            host.AddServiceEndpoint(typeof(ITest), webBinding, uriBase)
                            .Behaviors.Add(new WebHttpJsonNetBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            //////////////////////////////////////////////
            WebHttpBinding webBinding2 = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304,
                SendTimeout = TimeSpan.FromMinutes(4)
            };

            EndpointAddress endpoint = new EndpointAddress(baseAddress);
            
            ServiceLocal client = new ServiceLocal(webBinding2, endpoint);
            client.Endpoint.Behaviors.Add(new WebHttpJsonNetBehavior());
            
            var res2 = client.saveDataGet3(new InputData { FirstName = "myname2", LastName = "mylastname2" }, "saveDataGet3");
            // ok
            var res3 = client.saveDataGet4(new InputData { FirstName = "myname4", LastName = "mylastname4" }, "saveDataGet4");
            //
            var res4 = client.saveDataGet6(new InputData {FirstName = "fr", LastName = "lst"}, MyEnum.second);

            Console.WriteLine("######### risultato ########");
            //Console.WriteLine(res);
            Console.WriteLine(res2);
            Console.WriteLine(res3);
            Console.WriteLine(res4);
            Console.ReadLine();
        }


        public void RunServiceWithProxy2()
        {
            WebHttpBinding webBinding = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304
            };

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/jargs";
            Uri uriBase = new Uri(baseAddress);

            ServiceHost host = new ServiceHost(typeof(LocalService), uriBase);
            host.AddServiceEndpoint(typeof(ILocalService), webBinding, uriBase)
                            .Behaviors.Add(new WebHttpJsonNetBehavior2());

            host.Open();
            Console.WriteLine("Host opened");

            //////////////////////////////////////////////
            WebHttpBinding webBinding2 = new WebHttpBinding
            {
                ContentTypeMapper = new RawContentMapper(),
                MaxReceivedMessageSize = 4194304,
                MaxBufferSize = 4194304,
                SendTimeout = TimeSpan.FromMinutes(4)
            };

            EndpointAddress endpoint = new EndpointAddress(baseAddress);

            ProxyService client = new ProxyService(webBinding2, endpoint);
            //client.Endpoint.Behaviors.Add(new WebHttpJsonNetBehavior());
            client.Endpoint.Behaviors.Add(new NewHttpJsonNetBehavior());
            
            //var res = client.saveDataGet3(new InputData { FirstName = "myname", LastName = "mylastname" }, "my str");
            //var res1 = client.saveDataGet3(new InputData { FirstName = "myname", LastName = "mylastname" }, null);

            var res1 = client.ReadInputData1(new InputData { FirstName = "myname2", LastName = "mylastname2" }, "ReadInputData1");

            // ok
            //var res2 = client.ReadInputData2(new InputData { FirstName = "myname4", LastName = "mylastname4" }, "ReadInputData2");

            Console.WriteLine("######### risultato ########");
            //Console.WriteLine(res);
            Console.WriteLine(res1);
            //Console.WriteLine(res2);
            Console.ReadLine();
        }
    }
}
