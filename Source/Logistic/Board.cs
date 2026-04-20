using SimulationEngine.Source.Data.Commands;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using System;
using System.Drawing;

namespace SimulationEngine.Source.Logistic
{
    public class Board
    {
        int _width;
        int _height;
        Unit?[,] _tiles;
        Dictionary<Cell, HashSet<Unit>> _overlappingCells;
        public Board(int boardX, int boardY)
        {
            _width = boardX;
            _height = boardY;
            _tiles = new Unit?[_width, _height];
            _overlappingCells = new();
        }

        public void Clear()
        {
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    _tiles[i, j] = null;
            _overlappingCells.Clear();
        }

        public Point MirrorCoordinates(Point coords)
        {
            Point p = new();
            p.X = _width - 1 - coords.X;
            // height is multiplied by 2 in order to account for the doubled mirrored board of the enemy player
            p.Y = 2 * _height - 1 - coords.Y;
            return p;
        }

        public int Width => _width;
        public int Height => _height;
        public Cell Extend => new Cell(_width, _height);

        public bool IsInBounds(Cell coords) => Cell.IsCellInBoundsInclusive(coords, new(0,0), new(_width-1,_height-1));

        public Unit? Get(Cell coords) => _tiles[coords.x, coords.y];

        public HashSet<Unit> GetAll(Cell coords)
        {
            HashSet<Unit> units = new();
            Unit? currentUnit = Get(coords);
            if(currentUnit != null) units.Add(currentUnit);
            if(_overlappingCells.TryGetValue(coords, out HashSet<Unit> aditionalUnits)) units.UnionWith(aditionalUnits);
            return units;
        }

        public HashSet<Unit> GetAllInBoundsInclusive(Cell boundUpperLeft, Cell boundLowerRight)
        {
            HashSet<Unit> units = new();

            for(int x=boundUpperLeft.x; x<=boundLowerRight.x; x++)
            {
                for (int y = boundUpperLeft.y; y <= boundLowerRight.y; y++)
                {
                    units.UnionWith(GetAll(new(x, y)));
                }
            }
            return units;
        }

        public HashSet<Unit>? GetAdditional(Cell coords)
        {
            if(_overlappingCells.ContainsKey(coords)) return _overlappingCells[coords];
            return null;
        }

        public void Set(Cell coords, Unit? unit) => _tiles[coords.x, coords.y] = unit;

        public bool PlaceUnit(Cell coords, Unit unit)
        {
            Cell unitExtend = coords + unit.Ocupation.Extend;
            if (!IsInBounds(unitExtend + new Cell(-1, -1))) return false;

            for(int x = coords.x; x< unitExtend.x; x++)
            {
                for(int y=coords.y;y< unitExtend.y; y++)
                {
                    Cell step = new(x,y);
                    Unit? currentUnit = Get(step);
                    Set(step, unit);
                    if (currentUnit == null) continue;
                    if (!_overlappingCells.ContainsKey(step)) _overlappingCells.Add(step,new());
                    _overlappingCells[step].Add(currentUnit);
                }
            }
            unit.Position = coords;
            return true;
        }

        public bool RemoveUnit(Cell coords, Unit unit)
        {
            Cell pos = unit.Position;
            Cell unitExtend = pos + unit.Ocupation.Extend;
            if (!Cell.IsCellInBoundsInclusive(coords, pos, unitExtend + new Cell(-1,-1) )) return false;

            for (int x = pos.x; x < unitExtend.x; x++)
            {
                for (int y = pos.y; y < unitExtend.y; y++)
                {
                    Cell step = new(x, y);
                    Unit? currentUnit = Get(step);
                    if (currentUnit != unit)
                    {
                        if (!_overlappingCells.TryGetValue(step, out HashSet<Unit> aditionalUnits)) return false;
                        if (!aditionalUnits.Contains(unit)) return false;
                        aditionalUnits.Remove(unit);
                        continue;
                    }
                    Set(new(x,y), null);
                }
            }

            return true;
        }

        public Dictionary<Unit, Cell> SnapshotPositions()
        {
            Dictionary<Unit, Cell> snapshot = new();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cell pos = new Cell(x, y);
                    Unit? unit = Get(pos);
                    if (unit != null && !snapshot.ContainsKey(unit))
                        snapshot[unit] = pos;
                }
            }
            return snapshot;
        }

        public void RollbackPositions(Dictionary<Unit, Cell> snapshot)
        {
            Clear();
            foreach (KeyValuePair<Unit, Cell> kv in snapshot)
            {
                Unit unit = kv.Key;
                Cell anchor = kv.Value;
                for (int x = 0; x < unit.Ocupation.Width; x++)
                    for (int y = 0; y < unit.Ocupation.Height; y++)
                        Set(new Cell(anchor.x + x, anchor.y + y), unit);
            }
        }

        public void Print()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Unit? unit = Get(new Cell { x = x, y = y });
                    string id = unit == null ? "0" : unit.Id.ToString();
                    int numSpace = 3 - id.Length;
                    Console.Write($"| {id}");
                    for (int i = 0; i < numSpace; i++) Console.Write(" ");
                    Console.Write(" |");
                }
                Console.WriteLine();
                for (int x = 0; x < Width; x++) Console.Write("-------");
                Console.WriteLine();
            }
        }
    }
}