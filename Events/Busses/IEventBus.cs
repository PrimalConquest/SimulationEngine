using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Events.Busses
{
    internal interface IEventBus<T, P>
    {
        void RegisterChannel(T eventType);

        bool ClearChannel(T eventType);

        bool RemoveChannel(T eventType);

        bool AddListener(T eventType, EventCallback<P> callback, int priority = 0, bool enforceEventCreation = false);

        int Raise(T eventType, P payload);

        bool RemoveListener(T Listener, EventCallback<P> callback);

    }
}
