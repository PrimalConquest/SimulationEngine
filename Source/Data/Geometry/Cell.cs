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

        public Cell(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public Cell(uint X, uint Y)
        {
            x = (int)X;
            y = (int)Y;
        }

        public override string ToString() => $"[X:{x}, Y:{y}]";

        public override bool Equals(object obj) => obj is Cell other && x == other.x && y == other.y;
        public override int GetHashCode() => HashCode.Combine(x, y);

        public int Magnitude()
        {
            return x + y;
        }

        public uint MagnitudeAbs()
        {
            return (uint)Math.Abs(x + y);
        }

        public static Cell operator +(Cell left, Cell right)
        {
            return new Cell { x = left.x + right.x, y = left.y + right.y };
        }

        public static Cell operator *(Cell left, Cell right)
        {
            return new Cell { x = left.x * right.x, y = left.y * right.y };
        }

        public static Cell operator *(Cell left, int scale)
        {
            return new Cell { x = left.x * scale, y = left.y * scale };
        }

        public static Cell operator *(Cell left, uint scale)
        {
            return left * (int)scale;
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return left.x == right.x && left.y == right.y ;
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !(left == right);
        }

        public static Cell GetMoveDirection(EDirection direction)
        {

            switch (direction)
            {
                case EDirection.Up: return new(0, 1);
                case EDirection.Down: return new(0, -1);
                case EDirection.Left: return new(-1, 0);
                case EDirection.Right: return new(1, 0);
                default: return new(0, 0);
            }
        }

        public static bool IsCellInBounds(Cell check, Cell boundUpperLeft, Cell boundLowerRight)
        {
            return check.x > boundUpperLeft.x && check.x < boundLowerRight.x && check.y > boundUpperLeft.y && check.y < boundLowerRight.y;
        }

        public static bool IsCellInBoundsInclusive(Cell check, Cell boundUpperLeft, Cell boundLowerRight)
        {
            return check.x >= boundUpperLeft.x && check.x <= boundLowerRight.x && check.y >= boundUpperLeft.y && check.y <= boundLowerRight.y;
        }
    }
}
