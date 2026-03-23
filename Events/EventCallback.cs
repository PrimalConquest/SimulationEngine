using SimulationEngine.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Events
{
    public class EventCallback<P>
    {
        public int Priority { get; }
        public Action<EventPayload> Handle { get; }
        public bool IsOneShot { get; }

        public EventCallback(Action<EventPayload> handle, int priority = 0, bool isOneShot = false)
        {
            Priority = priority;
            Handle = handle;
            IsOneShot = isOneShot;
        }
    }
}
