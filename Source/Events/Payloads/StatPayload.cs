using SimulationEngine.Source.Enums.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    internal class StatPayload : EventPayload
    {
        public EValueType Type { get; set; }
        public int Value { get; set; }

        public StatPayload(EValueType type, int value)
        {
            Type = type; Value = value;
        }
    }
}
