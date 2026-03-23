using SimulationEngine.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Events
{
    public struct EventCallback<P>
    {
        public int priority;
        public Action<P> handler;
        public bool isOneShot;
    }
}
