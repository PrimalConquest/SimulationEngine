using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    public class ValueChangedPayload<T> : ValuePayload<T>
    {
        public T? OldValue { get; set; }
        public ValueChangedPayload(T newValue, T oldValue) : base(newValue)
        {
            OldValue = oldValue;
        }
    }
}
