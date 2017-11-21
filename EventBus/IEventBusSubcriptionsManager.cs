using EventBus.Abstractions;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBusSubcriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddDynamicSubcriptions<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void RemoveDynamicSubcription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        void AddSubcription<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>;
        void RemoveSubcription<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>;

        bool HasSubcriptionsForEvent<TE>() where TE : IntegrationEvent;
        bool HasSubcriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
