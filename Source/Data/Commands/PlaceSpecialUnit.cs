using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Commands
{
    internal class PlaceSpecialUnit : IGameCommand
    {

        Player _player;
        uint _unitId;
        Cell _pos;

        public PlaceSpecialUnit(Player player, uint unitId, Cell pos)
        {
            _player = player;
            _unitId = unitId;
            _pos = pos;
        }

        public bool CanExecute()
        {
            if (_player.PlacedSpecialUnits.Contains(_unitId)) return false;

            Shape unitShape = _player.Units[_unitId].Ocupation;
            Board board = _player.Board;

            if (board.Get(_pos) != 0) return false;

            if (unitShape.Offsets == null) return true;

            foreach (Cell extend in unitShape.Offsets)
            {
                if (board.Get(_pos) != 0) return false;
            }

            return true;
        }

        public void Execute()
        {
            Unit unit = _player.Units[_unitId];
            Shape shape = unit.Ocupation;
            unit.Position = _pos;
            _player.Board.Set(_pos, _unitId);

            if (shape.Offsets == null) return;

            foreach (Cell extend in shape.Offsets)
            {
                _player.Board.Set(extend, _unitId);
            }

            _player.PlacedSpecialUnits.Add(_unitId);
            //unit.Value.UnitEventBus.Raise(EUnitEvent.Draft, new DraftPayload(position));
        }
    }
}
