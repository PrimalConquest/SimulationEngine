using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Interfaces
{
    public interface IGameCommand
    {
        bool CanExecute();

        void Execute();
    }
}
