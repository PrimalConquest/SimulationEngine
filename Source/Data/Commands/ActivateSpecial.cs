using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Commands
{
    public class ActivateSpecial : IGameCommand
    {
        Player _player;
        uint _unitId;
        public ActivateSpecial(Player player, uint unitId)
        {
            _player = player;
            _unitId = unitId;
        }
        public bool CanExecute()
        {
            if(SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;

            if(!_player.BoardUnits.ContainsKey(_unitId)) return false;
            _player.BoardUnits.TryGetValue(_unitId, out Unit unit);
            if (unit == null) return false;
            return unit.CanActivate;
        }

        public void Execute()
        {
            _player.BoardUnits.TryGetValue(_unitId, out Unit unit);
            if (unit == null) return;
            unit.UnitEventBus.Raise(EUnitEvent.Activate, new());

            SimulationSystem.CheckStateChain();
        }
    }
}
