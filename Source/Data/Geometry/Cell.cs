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

        public int Magnitude()
        {
            return x + y;
        }

        public static Cell operator +(Cell left, Cell right)
        {
            return new Cell { x = left.x + right.x, y = left.y + right.y };
        }

        public static Cell operator *(Cell left, Cell right)
        {
            return new Cell { x = left.x * right.x, y = left.y * right.y };
        }
    }
}
