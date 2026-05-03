using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BattleComunication.Interfaces
{
    public interface ISimulationHub
    {
        bool SendCommand(ICommandInfo info);
        Task ReportMatchResultAsync(string winnerId, string loserId);
        void StartGame();
    }
}
