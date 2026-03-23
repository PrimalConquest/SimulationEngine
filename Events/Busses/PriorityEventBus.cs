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

        public bool RemoveChannel(ESimulationEvent eventType)
        {
            return _channels.Remove(eventType);
        }

        /// <summary>
        /// Adds the given Action to the invocation list.
        /// Actions with the save priority are LIFO ordered
        /// </summary>
        public bool AddListener(ESimulationEvent eventType, Action<EventPayload> callback, int priority = 0, bool enforceEventCreation = false)
        {
            if (!_channels.TryGetValue(eventType, out var list))
            {
                if (!enforceEventCreation)
                    return false;

                RegisterChannel(eventType);
            }

            // insert in sorted position — O(n) but list stays ordered
            // so Raise() never needs to sort
            int i = 0;
            while (i < list.Count && list[i].Priority <= priority) i++;
            list.Insert(i, listener);

            return true;
        }

        public bool Raise(ESimulationEvent eventType, EventPayload payload)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAllForListener(ESimulationEvent Listener)
        {
            throw new NotImplementedException();
        }

        public bool RemoveListener(ESimulationEvent Listener, Action<EventPayload> callback)
        {
            throw new NotImplementedException();
        }
    }
}
