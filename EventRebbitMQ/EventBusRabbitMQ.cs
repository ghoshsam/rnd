using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using EventBus;
using System;
using Autofac;
using RabbitMQ.Client;
using EventBus.Events;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using Polly;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace EventRebbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "my_event_bus";

        private readonly IRabbitMQPersistentConnection _presistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubcriptionsManager _subcriptionsManager;
        private readonly ILifetimeScope _autofac_lifetimeScope;
        private readonly string AUTOFAC_SCOPE_NAME = "my_event_bus";

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
                                ILogger<EventBusRabbitMQ> logger,
                                IEventBusSubcriptionsManager subcriptionsManager,
                                ILifetimeScope lifetimeScope_autofac)
        {

            _presistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subcriptionsManager = subcriptionsManager ?? new InMemoryEventBusSubscriptionsManager();
            _consumerChannel = CreateConsumerChannel();
            _autofac_lifetimeScope = lifetimeScope_autofac;

            _subcriptionsManager.OnEventRemoved += _subcriptionsManager_OnEventRemoved;

        }

        private IModel CreateConsumerChannel()
        {
            if (!_presistentConnection.IsConnected)
            {
                _presistentConnection.TryConnect();
            }

            var channel = _presistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);
                await ProcessEvent(eventName, message);
            };


            // need to check 
            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);


            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();

            };

            return channel;

        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subcriptionsManager.HasSubcriptionsForEvent(eventName))
            {
                using(var scope = _autofac_lifetimeScope.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subcriptions = _subcriptionsManager.GetHandlersForEvent(eventName);
                    foreach(var subcription in subcriptions)
                    {
                        if (subcription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subcription.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await  handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subcription.HandlerType);
                            var eventType = _subcriptionsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
        }

        private void _subcriptionsManager_OnEventRemoved(object sender, string eventName)
        {
            if (_presistentConnection.IsConnected)
            {
                _presistentConnection.TryConnect();
            }

            using(var channel = _presistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);
                if (_subcriptionsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }

            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_presistentConnection.IsConnected)
            {
                _presistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                        .Or<SocketException>()
                        .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {
                            _logger.LogWarning(ex.ToString());
                        });

            using(var channel = _presistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                channel.ExchangeDeclare(exchange: BROKER_NAME,
                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                policy.Execute(() =>
                {

                    channel.BasicPublish(exchange: BROKER_NAME,
                                        routingKey: eventName,
                                        basicProperties: null,
                                        body: body);
                });
            }
        }

        public void Subscribe<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>
        {
            var eventName = _subcriptionsManager.GetEventKey<TE>();
            DoInternalSubcription(eventName);
            _subcriptionsManager.AddSubcription<TE, TH>();
        }
        
        private void DoInternalSubcription(string eventName)
        {
            var _containsKey = _subcriptionsManager.HasSubcriptionsForEvent(eventName);
            if (!_containsKey)
            {
                if (!_presistentConnection.IsConnected)
                {
                    _presistentConnection.TryConnect();
                }
                using(var channel = _presistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                        exchange: BROKER_NAME,
                        routingKey: eventName);
                }
            }
        }
      

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subcriptionsManager.RemoveDynamicSubcription<TH>(eventName);
        }

        public void Unsubscribe<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>
        {
            _subcriptionsManager.RemoveSubcription<TE, TH>();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoInternalSubcription(eventName);
            _subcriptionsManager.AddDynamicSubcriptions<TH>(eventName);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            _subcriptionsManager.Clear();
        }

    }
}
