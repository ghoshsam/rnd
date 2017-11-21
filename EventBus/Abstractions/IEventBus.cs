using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Subscribe<TE, TH>() where TE : IntegrationEvent where TH : IIntegrationEventHandler<TE>;

        void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<TE, TH>() where TE : IntegrationEvent where TH : IIntegrationEventHandler<TE>;

        void Publish(IntegrationEvent @event);
    }
}
