using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventRebbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Received.IntegrationEvents.Event;
using Received.IntegrationEvents.Handler;
using System;
using System.Text;

namespace Received
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
            RegisterEvents(serviceCollection);
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(serviceCollection);
            var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
            configerEventBus(serviceProvider);

          

        }

        private static void configerEventBus(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<CustomerCreateIntegrationEvent, CustomerCreateIntegrationEventHandler>();

        }

        private static void RegisterEvents(IServiceCollection serviceProvider)
        {
            serviceProvider.AddTransient<CustomerCreateIntegrationEventHandler>();
        }
        //public static void Main(string[] args)
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "hello",
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        var consumer = new EventingBasicConsumer(channel);
        //        consumer.Received += (model, ea) =>
        //        {
        //            var body = ea.Body;
        //            var message = Encoding.UTF8.GetString(body);
        //            Console.WriteLine(" [x] Received {0}", message);
        //        };
        //        channel.BasicConsume(queue: "hello",
        //                             autoAck: true,
        //                             consumer: consumer);

        //        Console.WriteLine(" Press [enter] to exit.");
        //        Console.ReadLine();
        //    }
        //    //Console.WriteLine("Hello World!");
        //}
    }
}
