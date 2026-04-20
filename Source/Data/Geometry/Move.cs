using SimulationEngine.Source.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Geometry
{
    public struct Move
    {
        public uint Step { get; set; }
        public EDirection Direction { get; set; }

        public Move(uint step, EDirection direction)
        {
            Step = step;
            Direction = direction;
        }
    }
}
