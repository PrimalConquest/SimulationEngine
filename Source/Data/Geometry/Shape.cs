using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums;
using System.Collections.Generic;

namespace SimulationEngine.Source.Data.Geometry
{
    public struct Shape
    {
        public uint Width  { get; set; }
        public uint Height { get; set; }

        public Shape()
        {
            Width  = 1;
            Height = 1;
        }

        public Shape(uint width, uint height)
        {
            Width  = width;
            Height = height;
        }

        public IEnumerable<Cell> GetOffsets()
        {
            for (int dy = 0; dy < Height; dy++)
                for (int dx = 0; dx < Width; dx++)
                    yield return new Cell { x = dx, y = dy };
        }

        public Cell Extend()
        {
            return new Cell { x = (int)Width, y = (int)Height };
        }

        public (Cell, Cell, Cell) GetWall(EDirection direction)
        {
            switch(direction)
            {
                case EDirection.Left: return (new Cell { x = 0, y = 0 }, new Cell { x = 0, y = (int)Height-1 }, Cell.GetMoveDirection(EDirection.Up));
                case EDirection.Right: return (new Cell { x = (int)Width-1, y = 0 }, new Cell { x = (int)Width-1, y = (int)Height-1 }, Cell.GetMoveDirection(EDirection.Up));
                case EDirection.Up: return (new Cell { x = 0, y = 0 }, new Cell { x = (int)Width-1, y = 0 }, Cell.GetMoveDirection(EDirection.Right));
                case EDirection.Down: return (new Cell { x = 0, y = (int)Height-1 }, new Cell { x = (int)Width-1, y = (int)Height-1 }, Cell.GetMoveDirection(EDirection.Right));
                default: return (new Cell { x = 0, y = 0 }, new Cell { x = 0, y = 0 }, Cell.GetMoveDirection(EDirection.Right));
            }
        }

    }
}
