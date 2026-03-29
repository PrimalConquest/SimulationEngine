using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Busses.Interfaces
{
    internal interface IEventBus<T> : IBus<T, EventPayload> { }
}
