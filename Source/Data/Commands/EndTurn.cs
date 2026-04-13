using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SimulationEngine.Source.Data.Commands
{
    public class EndTurn : IGameCommand
    {
        Player _player;

        public EndTurn(Player playerEnding)
        {
            _player = playerEnding;
        }
        public bool CanExecute()
        {
            return SimulationSystem.ActiveGame.CurrentPlayer == _player;
        }
        public void Execute()
        {
            SimulationSystem.ActiveGame.EndTurn();
            SimulationSystem.CheckStateChain();
        }
    }
}
