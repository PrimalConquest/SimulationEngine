using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    public class ValuePayload<T> : EventPayload
    {
        public T? Value { get; set; }

        public ValuePayload(T? value)
        {
            Value = value;
        }
    }
}
