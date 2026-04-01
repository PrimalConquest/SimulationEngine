using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Interfaces.Events
{
    internal interface IEventBus<T, P> where P : EventPayload
    {
        public void RegisterChannel(T eventType);

        bool ClearChannel(T eventType);

        bool RemoveChannel(T eventType);

        bool AddListener(T eventType, EventCallback<P> callback, bool enforceEventCreation = false);

        int Raise(T eventType, P payload);

        bool RemoveListener(T Listener, EventCallback<P> callback);

    }
}
