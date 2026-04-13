using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace SimulationEngine.Source.Events.Busses
{
    internal class PriorityEventBus<T, P> : IEventBus<T, P> where P : EventPayload 
    {
        private readonly Dictionary<T, List<EventCallback<P>>> _channels = new();

        public void RegisterChannel(T eventType)
        {
            if (!_channels.ContainsKey(eventType))
                _channels[eventType] = new();
        }

        public bool ClearChannel(T eventType)
        {
            if (!_channels.TryGetValue(eventType, out var list))
            {
                string msg = (eventType == null) ? "null" : eventType.ToString();
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"PriorityEventBus:ClearChannel Trying to listen on value type that isnt registerd ({msg})");
                return false;
            }

            list.Clear();
            return true;
        }

        public bool RemoveChannel(T eventType)
        {
            return _channels.Remove(eventType);
        }

        /// <summary>
        /// Adds the given Action to the invocation list.
        /// Actions with the same priority are LIFO ordered
        /// </summary>
        public bool AddListener(T eventType, EventCallback<P> callback, bool enforceEventCreation = false)
        {
            List<EventCallback<P>> list;
            if (!_channels.TryGetValue(eventType, out list))
            {
                if (!enforceEventCreation)
                {
                    string msg = (eventType == null) ? "null" : eventType.ToString();
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"PriorityEventBus:AddListener Trying to add listener on value type that isnt registerd ({msg}) without enfortcing event");
                    return false;
                }

                RegisterChannel(eventType);
                _channels.TryGetValue(eventType, out list);
            }

            int i = 0;
            for (; i < list.Count && list[i].Priority < callback.Priority; i++);
            list.Insert(i, callback);

            return true;
        }

        public bool RemoveListener(T eventType, EventCallback<P> callback)
        {
            if (!_channels.TryGetValue(eventType, out var list))
            {
                string msg = (eventType == null) ? "null" : eventType.ToString();
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"PriorityEventBus:RemoveListener Trying to remove listener on value type that isnt registerd ({msg})");
                return false;
            }

            int removed = list.RemoveAll(l => l == callback);
            return removed > 0;
        }

        public int Raise(T eventType, P payload)
        {
            if (!_channels.TryGetValue(eventType, out var list))
            {
                string msg = (eventType == null) ? "null" : eventType.ToString();
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"PriorityEventBus:Raise Trying to raise event on value type that isnt registerd ({msg})");
                return -1;
            }

            var snapshot = list.ToArray();
            int invocations = 0;

            foreach (var listener in snapshot)
            {
                if (payload.Cancelled) break;

                listener.Handle(payload);
                invocations++;

                if (listener.IsOneShot)
                    list.Remove(listener);
            }

            return invocations;
        }

    }
}
