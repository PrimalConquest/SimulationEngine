using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Commands
{
    internal class Move : IGameCommand
    {
        Player _player;
        Cell _pos;
        Cell _direction;
        int _moveCost;
        int _maxRepetitions;
        public Move(Player player, Cell pos, EDirection direction)
        {
            _player = player;
            _pos = pos;
            _direction = GetMoveDirection(direction);
            _player.Units.TryGetValue(_player.Board.Get(_pos), out Unit unit);
            if (unit == null) { return; }
            _maxRepetitions = unit.GetStat(EStat.MoveSpeed);
            _moveCost = unit.GetStat(EStat.MoveCost);
        }

        public bool CanExecute()
        {
            if (_player.CurrentMoves < _moveCost) return false;

            Board board = _player.Board;

            _player.Units.TryGetValue(board.Get(_pos), out Unit unit);

            if(unit == null) { return false; }

            _player.Units.TryGetValue(board.Get(_pos+_direction), out Unit swapUnit);

            if (!unit.CanMove || !swapUnit.CanMove) return false;

            if (unit.Ocupation.Offsets == null) return true;

            foreach(Cell tile in unit.Ocupation.Offsets)
            {
                _player.Units.TryGetValue(board.Get(_pos + tile), out Unit unitOffset);
                _player.Units.TryGetValue(board.Get(_pos + tile + _direction), out Unit swapUnitOffset);

                if (!unitOffset.CanMove || !swapUnitOffset.CanMove) return false;
            }

            return true;
        }

        public void Execute()
        {
            _player.CurrentMoves -= _moveCost;
            _moveCost = 0;

            Board board = _player.Board;
            _player.Units.TryGetValue(board.Get(_pos), out Unit unit);
            if (unit == null) { return; }

            Cell unitAnchor = unit.Position;

            List <Cell> shapeSnapshot = new List<Cell>();
            shapeSnapshot.Add(new Cell { x = 0, y = 0 });
            if(unit.Ocupation.Offsets!=null) shapeSnapshot.AddRange(unit.Ocupation.Offsets);

            shapeSnapshot.OrderByDescending(cell => (cell * _direction).Magnitude());

            foreach(Cell tile in shapeSnapshot)
            {
                uint current = board.Get(unitAnchor + tile);
                uint swapWith = board.Get(unitAnchor + tile + _direction);

                board.Set(unitAnchor + tile, swapWith);
                board.Set(unitAnchor + tile + _direction, swapWith);
            }

            _maxRepetitions -= 1;

            if(_maxRepetitions>0 && CanExecute()) Execute();
        }

        Cell GetMoveDirection(EDirection direction)
        {

            switch(direction)
            {
                case EDirection.Up : return new Cell { x = 0, y = 1 };
                case EDirection.Down: return new Cell { x = 0, y = -1 };
                case EDirection.Left: return new Cell { x = -1, y = 0 };
                case EDirection.Right: return new Cell { x = 1, y = 0 };
                default: return new Cell { x = 0, y = 0 };
            }
        }
    }
}
