using System;
using System.Collections.Generic;
using System.Text;

namespace SharedUtils.Source.Events
{
    public interface IEventBus<T, P>
    {
        public void RegisterChannel(T eventType);

        bool ClearChannel(T eventType);

        bool RemoveChannel(T eventType);

        bool AddListener(T eventType, EventCallback<P> callback, bool enforceEventCreation = false);

        int Raise(T eventType, P payload);

        bool RemoveListener(T Listener, EventCallback<P> callback);

    }
}
