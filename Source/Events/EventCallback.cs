using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events
{
    public class EventCallback<P>
    {
        public int Priority { get; }
        public Action<P> Handle { get; }
        public bool IsOneShot { get; }

        public EventCallback(Action<P> handle, int priority = 5, bool isOneShot = false)
        {
            Priority = priority;
            Handle = handle;
            IsOneShot = isOneShot;
        }

        public static implicit operator EventCallback<P>( Action<P> handle) => new(handle);
    }
}
