using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Commands
{
    internal class Move : IGameCommand
    {
        Player _player;
        uint _unitId;
        Cell _direction;
        int _moveCost;
        int _maxRepetitions;
        public Move(Player player, Cell pos, EDirection direction)
        {
            _player = player;
            _unitId = _player.Board.Get(pos);
            _direction = Cell.GetMoveDirection(direction);
            _player.BoardUnits.TryGetValue(_unitId, out Unit unit);
            if (unit == null) { return; }
            _maxRepetitions = unit.GetStat(EStat.MoveSpeed);
            _moveCost = unit.GetStat(EStat.MoveCost);
        }

        public bool CanExecute()
        {
            if (_player.CurrentMoves < _moveCost) return false;

            Board board = _player.Board;
            _player.BoardUnits.TryGetValue(_unitId, out Unit unit);

            if(unit == null) { return false; }

            if (!unit.CanMove) return false;

            Cell unitAnchor = unit.Position;

            List<Cell> shapeSnapshot = new List<Cell>();
            shapeSnapshot.Add(new Cell { x = 0, y = 0 });
            if (unit.Ocupation.Offsets != null) shapeSnapshot.AddRange(unit.Ocupation.Offsets);

            shapeSnapshot = shapeSnapshot.OrderByDescending(cell => (cell * _direction).Magnitude()).ToList();

            foreach (Cell tile in shapeSnapshot)
            {
                Cell next = unitAnchor + tile + _direction;
                if(!board.IsInBounds(next)) return false;
                uint swapUnitId = board.Get(next);
                if(swapUnitId == 0 || _unitId == swapUnitId) continue;

                _player.BoardUnits.TryGetValue(swapUnitId, out Unit swapUnitOffset);

                if (swapUnitOffset!=null && !swapUnitOffset.CanDisplace) return false;
            }

            return true;
        }

        public void Execute()
        {
            _player.CurrentMoves -= _moveCost;
            _moveCost = 0;

            Board board = _player.Board;
            _player.BoardUnits.TryGetValue(_unitId, out Unit unit);
            if (unit == null) { return; }

            Cell unitAnchor = unit.Position;

            List <Cell> shapeSnapshot = new List<Cell>();
            shapeSnapshot.Add(new Cell { x = 0, y = 0 });
            if(unit.Ocupation.Offsets!=null) shapeSnapshot.AddRange(unit.Ocupation.Offsets);

            shapeSnapshot = shapeSnapshot.OrderByDescending(cell => (cell * _direction).Magnitude()).ToList();

            foreach(Cell tile in shapeSnapshot)
            {
                Cell currentCell = unitAnchor + tile;
                Cell swapCell = unitAnchor + tile + _direction;

                uint current = board.Get(currentCell);
                uint swapWith = board.Get(swapCell);

                if (current == swapWith) continue;

                board.Set(currentCell, swapWith);
                board.Set(swapCell, current);

                SimulationSystem.CheckForMatchPositions.Add(currentCell);
                SimulationSystem.CheckForMatchPositions.Add(swapCell);

                if (swapWith == 0) continue;
                _player.BoardUnits.TryGetValue(swapWith, out Unit displacedUnit);
                if(displacedUnit!=null) displacedUnit.UnitEventBus.Raise(EUnitEvent.Displace, new ValueChangedPayload<Cell>(currentCell, swapCell));
            }

            unit.UnitEventBus.Raise(EUnitEvent.Move, new ValueChangedPayload<Cell>(unitAnchor+_direction, unitAnchor));

            _maxRepetitions -= 1;

            if(_maxRepetitions>0 && CanExecute()) Execute();
            else SimulationSystem.CheckStateChain();
        }


    }
}
