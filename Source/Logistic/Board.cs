using SimulationEngine.Source.Data.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    internal class Board
    {
        int _width;
        int _height;
        int[,] _tiles;

        public Board(int boardX, int boardY)
        {
            _tiles = new int[_width, _height];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j=0; j < _height; i++)
                {
                    _tiles[i,j] = 0;
                }
            }
        }
        
        public Point MirrorCoordinates(Point coords)
        {
            Point p = new();

            p.X = _width - 1 - coords.X;
            p.Y = 2 * _height - 1 - coords.Y;

            return p;
        }
        public int Get(Point coords)
        {
            return _tiles[coords.X, coords.Y];
        }

        public void Set(Point coords, int index)
        {
            _tiles[coords.X, coords.Y] = index;
        }
    }
}
