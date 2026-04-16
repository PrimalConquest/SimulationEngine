using SimulationEngine.Source.Data.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Enums
{
    public enum EDirection : byte
    {
        Up,
        Down,
        Left,
        Right
    }
    public static class DirectionExtensions
    {

        extension(EDirection direction)
        {
            public EDirection Opposite()
            {
                switch(direction)
                {
                    case EDirection.Left: return EDirection.Right;
                    case EDirection.Right: return EDirection.Left;
                    case EDirection.Up: return EDirection.Down;
                    case EDirection.Down: return EDirection.Up;
                    default: return direction;
                }

            }

            public Cell ToVector()
            {
                return Cell.GetMoveDirection(direction);

            }
        }
    }
}
