using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Data.Commands
{
    public class ActivateSpecial : IGameCommand
    {
        Player _player;
        string _unitId;

        public ActivateSpecial(Player player, string unitId)
        {
            _player = player;
            _unitId = unitId;
        }

        public bool CanExecute()
        {
            if (SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;

            if (SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;

            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return false;

            if (!_player.BoardUnits.Contains(unit))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"ActivateSpecial.CanExecute - unit [{_unitId}] is not on the board (not drafted)");
                return false;
            }

            return unit.CanActivate;
        }

        public void Execute()
        {
            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return;

            unit.UnitEventBus.Raise(EUnitEvent.Activate, new EventPayload());

            SimulationSystem.CheckStateChain();
        }
    }
}
