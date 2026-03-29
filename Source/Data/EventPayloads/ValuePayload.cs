using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.EventPayloads
{
    internal class ValuePayload<T> : EventPayload
    {
        public T Value { get; set; }

        public ValuePayload(T value)
        {
            Value = value;
        }
    }
}
