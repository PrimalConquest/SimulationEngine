using SimulationEngine.Source.Enums.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    internal class StatPayload : EventPayload
    {
        public EStat Stat { get; set; }
        public uint Value { get; set; }

        public StatPayload(EStat stat, uint value)
        {
            Stat = stat;
            Value = value;
        }
    }
}
