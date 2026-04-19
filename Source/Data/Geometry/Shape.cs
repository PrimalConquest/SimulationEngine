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

        public Cell Extend
        {
            get { return new(Width, Height); }
        }

        public (Cell startOffset, Cell endOffset, Cell step) GetWall(EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Left: return (startOffset: new(0, 0), endOffset: new(0, Height - 1), Cell.GetMoveDirection(EDirection.Up));

                case EDirection.Right: return (startOffset: new(Width - 1, 0), endOffset: new(Width - 1, Height - 1), Cell.GetMoveDirection(EDirection.Up));

                case EDirection.Down: return (startOffset: new(0, 0), endOffset: new(Width - 1, 0), Cell.GetMoveDirection(EDirection.Right));

                case EDirection.Up: return (startOffset: new(0, Height - 1), endOffset: new(Width - 1, Height - 1), Cell.GetMoveDirection(EDirection.Right));

                default: return (startOffset: new(0, 0), endOffset: new(0, 0), Cell.GetMoveDirection(EDirection.Right));
            }
        }

    }
}
