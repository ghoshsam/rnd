using System;
using System.Collections.Generic;
using System.Text;
using EventBus.Abstractions;
using EventBus.Events;
using System.Linq;

namespace EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager:IEventBusSubcriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }
        public bool IsEmpty => !_handlers.Keys.Any();
        
        public event EventHandler<string> OnEventRemoved;

        public void AddDynamicSubcriptions<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubcription(typeof(TH), eventName, isDynamic: true);
        }
               

        public void AddSubcription<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>
        {
            var eventName = GetEventKey<TE>();
            DoAddSubcription(typeof(TH), eventName, isDynamic: false);
            _eventTypes.Add(typeof(TE));
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved(this, eventName);
            }
        }
        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }
        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }
        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);

        }
        public bool HasSubcriptionsForEvent<TE>() where TE : IntegrationEvent
        {
            var key = GetEventKey<TE>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubcriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public void RemoveDynamicSubcription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void RemoveSubcription<TE, TH>()
            where TE : IntegrationEvent
            where TH : IIntegrationEventHandler<TE>
        {
            var handlerToRemove = FindSubscriptionToRemove<TE, TH>();
            var eventName = GetEventKey<TE>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo handlerToRemove)
        {
            if (handlerToRemove != null)
            {
                _handlers[eventName].Remove(handlerToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }

            }
        }

        private void DoAddSubcription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            if (isDynamic)
            {
                _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }
    }
}
