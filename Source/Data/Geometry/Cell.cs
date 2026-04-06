using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Geometry
{
    public struct Cell
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return $"[X:{x}, Y:{y}]";
        }
    }
}
