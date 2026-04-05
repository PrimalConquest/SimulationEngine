using SimulationEngine.Source.Enums.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    internal class StatChangedPayload : StatPayload
    {
        public int OldValue { get; set; }
        public StatChangedPayload(EValueType type, int newValue, int oldValue) : base(type, newValue)
        {
            OldValue = oldValue;
        }
    }
}
