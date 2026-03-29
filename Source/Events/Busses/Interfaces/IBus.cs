using SimulationEngine.Source.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Busses.Interfaces
{
    internal interface IBus<T, P>
    {
        public void RegisterChannel(T eventType);

        bool ClearChannel(T eventType);

        bool RemoveChannel(T eventType);

        bool AddListener(T eventType, EventCallback<P> callback, int priority = 0, bool enforceEventCreation = false);

        int Raise(T eventType, P payload);

        bool RemoveListener(T Listener, EventCallback<P> callback);

    }
}
