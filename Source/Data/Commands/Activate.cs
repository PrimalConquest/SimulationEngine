using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Commands
{
    internal class Activate : IGameCommand
    {
        Player _player;
        uint _unitId;
        public Activate(Player player, uint unitId)
        {
            _player = player;
            _unitId = unitId;
        }
        public bool CanExecute()
        {
            if(!_player.PlacedSpecialUnits.Contains(_unitId)) return false;
            _player.Units.TryGetValue(_unitId, out Unit unit);
            if (unit == null) return false;
            return unit.CanActivate;
        }

        public void Execute()
        {
            _player.Units.TryGetValue(_unitId, out Unit unit);
            if (unit == null) return;
            unit.UnitEventBus.Raise(EUnitEvent.Activate, new());
        }
    }
}
