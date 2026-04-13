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
        bool _isCommander;
        uint _unitId;
        Cell _pos;

        public PlaceSpecialUnit(Player player, uint unitId, Cell pos)
        {
            _player = player;
            _isCommander = unitId == player.CommanderId;
            _unitId = unitId;
            _pos = pos;
        }

        public bool CanExecute()
        {
            if (_player.BoardUnits.ContainsKey(_unitId)) return false;

            if(!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return false;
 
            if (!unit.CanBeDrafted) return false;

            Board board = _player.Board;
            Dictionary<uint, Unit> units = _player.BoardUnits;

            Shape shape = unit.Ocupation;

            List<Cell> shapeSnapshot = new();
            shapeSnapshot.Add(new Cell { x = 0, y = 0 });
            if (shape.Offsets != null) shapeSnapshot.AddRange(shape.Offsets);


            foreach (Cell extend in shapeSnapshot)
            {
                if (!board.IsInBounds(_pos + extend)) return false;
                uint unitId = board.Get(_pos + extend);
                if (unitId != 0)
                {
                    if (!units.TryGetValue(unitId, out Unit occupant) || !occupant.CanBeOverriden) return false;
                }
            }

            return true;
        }

        public void Execute()
        {
            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return;

            Shape shape = unit.Ocupation;
            unit.Position = _pos;

            List<Cell> shapeSnapshot = new();
            shapeSnapshot.Add(new Cell { x=0, y=0});
            if (shape.Offsets != null) shapeSnapshot.AddRange(shape.Offsets);
            

            foreach (Cell extend in shapeSnapshot)
            {
                uint overridenId = _player.Board.Get(_pos + extend);
                _player.BoardUnits.TryGetValue(overridenId, out Unit overriden);
                _player.Board.Set(_pos + extend, _unitId);
                if (overriden != null) overriden.UnitEventBus.Raise(EUnitEvent.Override, new()); //may be give the overriding unit and/or position
                _player.BoardUnits.Remove(overridenId);
            }

            _player.BoardUnits.Add(_unitId, unit);
            unit.UnitEventBus.Raise(EUnitEvent.Draft, new DraftPayload(_pos));
        }
    }
}
