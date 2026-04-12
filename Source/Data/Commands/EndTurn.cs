using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SimulationEngine.Source.Data.Commands
{
    internal class EndTurn : IGameCommand
    {
        Game _currentGame;

        public EndTurn(Game currentGame)
        {
            _currentGame = currentGame;
        }
        public bool CanExecute()
        {
            return true;
        }
        public void Execute()
        {
            _currentGame.EndTurn();
            SimulationSystem.CheckStateChain();
        }
    }
}
