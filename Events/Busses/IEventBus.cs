using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Events.Busses
{
    internal interface IEventBus<T, P>
    {
        void RegisterChannel(T eventType);

        bool RemoveChannel(T eventType);

        bool AddListener(T eventType, EventCallback<P> callback, int priority = 0, bool enforceEventCreation = false);

        bool Raise(T eventType, P payload);

        bool RemoveListener(T Listener, Action<P> callback);

        bool RemoveAllForListener(T Listener);

    }
}
