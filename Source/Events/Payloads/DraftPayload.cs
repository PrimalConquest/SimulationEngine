using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    internal class DraftPayload : EventPayload
    {
        public Cell Position { get; set; }

        public DraftPayload(Cell position)
        {
            Position = position;
        }
    }
}
