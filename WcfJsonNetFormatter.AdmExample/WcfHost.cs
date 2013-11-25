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

namespace WcfJsonService.Example
{
    public class WcfHost
    {
        static void Main()
        {
            WcfHost host = new WcfHost();
            
            host.Initialize();
            host.Run();
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
            Console.WriteLine("Run a ServiceHost via administrative configuration...");
            Console.WriteLine();

            using (ServiceHost serviceHost = new ServiceHost(typeof(SalesService)))
            {
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
    }
}
