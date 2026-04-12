using SimulationEngine.Source.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Geometry
{
    public struct Cell
    {
        public int x;
        public int y;

        public override string ToString() => $"[X:{x}, Y:{y}]";

        public override bool Equals(object obj) => obj is Cell other && x == other.x && y == other.y;
        public override int GetHashCode() => HashCode.Combine(x, y);

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

        public static Cell GetMoveDirection(EDirection direction)
        {

            switch (direction)
            {
                case EDirection.Up: return new Cell { x = 0, y = 1 };
                case EDirection.Down: return new Cell { x = 0, y = -1 };
                case EDirection.Left: return new Cell { x = -1, y = 0 };
                case EDirection.Right: return new Cell { x = 1, y = 0 };
                default: return new Cell { x = 0, y = 0 };
            }
        }
    }
}
