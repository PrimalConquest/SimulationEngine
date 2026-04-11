using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    public class Board
    {
        int _width;
        int _height;
        uint[,] _tiles;

        public Board(int boardX, int boardY)
        {
            _width = boardX;
            _height = boardY;
            _tiles = new uint[_width, _height];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j=0; j < _height; j++)
                {
                    _tiles[i,j] = 0;
                }
            }
        }
        
        public Point MirrorCoordinates(Point coords)
        {
            Point p = new();

            p.X = _width - 1 - coords.X;
            //height is multiplied by 2 in order to account for the boubled mirrored board of the enmey player
            p.Y = 2 * _height - 1 - coords.Y;

            return p;
        }
        public bool IsInBounds(Cell coords) =>
            coords.x >= 0 && coords.x < _width && coords.y >= 0 && coords.y < _height;

        public uint Get(Cell coords)
        {
            return _tiles[coords.x, coords.y];
        }

        public void Set(Cell coords, uint index)
        {
            _tiles[coords.x, coords.y] = index;
        }
    }
}
