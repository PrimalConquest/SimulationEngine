using SimulationEngine.Events.EventTypes;
using SimulationEngine.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace SimulationEngine.Events.Busses
{
    internal class PriorityEventBus : IEventBus<ESimulationEvent, EventPayload>
    {
        private readonly Dictionary<ESimulationEvent, List<EventCallback<EventPayload>>> _channels = new();

        public void RegisterChannel(ESimulationEvent eventType)
        {
            if (!_channels.ContainsKey(eventType))
                _channels[eventType] = new();
        }

        public bool ClearChannel(ESimulationEvent eventType)
        {
            if (!_channels.TryGetValue(eventType, out var list))
                return false;

            list.Clear();
            return true;
        }

        public bool RemoveChannel(ESimulationEvent eventType)
        {
            return _channels.Remove(eventType);
        }

        /// <summary>
        /// Adds the given Action to the invocation list.
        /// Actions with the same priority are LIFO ordered
        /// </summary>
        public bool AddListener(ESimulationEvent eventType, EventCallback<EventPayload> callback, int priority = 0, bool enforceEventCreation = false)
        {
            if (!_channels.TryGetValue(eventType, out var list))
            {
                if (!enforceEventCreation)
                    return false;

                RegisterChannel(eventType);
            }

            int i = 0;
            for (; i < list.Count && list[i].Priority < priority; i++);
            list.Insert(i, callback);

            return true;
        }

        public bool RemoveListener(ESimulationEvent eventType, EventCallback<EventPayload> callback)
        {
            if (!_channels.TryGetValue(eventType, out var list))
                return false;

            int removed = list.RemoveAll(l => l == callback);
            return removed > 0;
        }

        public int Raise(ESimulationEvent eventType, EventPayload payload)
        {
            if (!_channels.TryGetValue(eventType, out var list))
                return -1;

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
