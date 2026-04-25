using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedServices.Source.Interfaces
{
    public interface ISimulationClient
    {
        bool RecieveCommand(ICommandInfo info);
        void ReceiveGameSetup();
    }
}
