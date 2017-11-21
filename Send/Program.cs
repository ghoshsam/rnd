using System;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Logging;
using EventRebbitMQ;
using EventBus.Abstractions;
using EventBus;
using Send.IntegrationEvents.Event;

namespace Send
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            serviceCollection.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost"//Configuration["EventBusConnection"]
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            serviceCollection.AddSingleton<IEventBus, EventBusRabbitMQ>();
            serviceCollection.AddSingleton<IEventBusSubcriptionsManager, InMemoryEventBusSubscriptionsManager>();

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(serviceCollection);
            var serviceProvider= new AutofacServiceProvider(containerBuilder.Build());

            //var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            // eventBus.

            
            CustomerCreateEventExecute(serviceProvider);
        }

        private static void CustomerCreateEventExecute(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            var eventMessage = new CustomerCreateIntegrationEvent("Sam", "Ghosh", "samarendra@insync.co.in");
            eventBus.Publish(eventMessage);
        }
        //public static void Main(string[] args)
        // {
        //     Console.WriteLine("Enter your message");




        //         var factory = new ConnectionFactory() { HostName = "localhost" };
        //         using (var connection = factory.CreateConnection())
        //         using (var channel = connection.CreateModel())
        //         {
        //             channel.QueueDeclare(queue: "hello",
        //                                  durable: false,
        //                                  exclusive: false,
        //                                  autoDelete: false,
        //                                  arguments: null);

        //             //string message = "Hello World";

        //             int result;
        //         //for (int i = 0; i < 10; i++)
        //         //{
        //         while (true)
        //         {
        //             result = Console.Read();
        //             string message =Convert.ToString( Convert.ToChar(result));
        //             var body = Encoding.UTF8.GetBytes(message);
        //             channel.BasicPublish(exchange: "",
        //                               routingKey: "hello",
        //                               basicProperties: null,
        //                               body: body);
        //         }

        //                 //Console.WriteLine("[x] Send {0}-{1}", i, message);
        //             //}


        //         }

        //    // Console.WriteLine("Press [enter] to exit.");
        //    // Console.ReadLine();
        //     //Console.WriteLine("Hello World!");
        // }
    }
}
