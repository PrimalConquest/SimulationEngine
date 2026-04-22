using Microsoft.AspNetCore.SignalR;
using SharedServices;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;

namespace BattleServer.Source.Hubs
{
    public class SimulationHub : Hub<ISimulationClient>
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public bool SendCommand(ICommandInfo info)
        {

            //Clients.All.RecieveCommand(info);

            return true;
        }

        public void ConnetToGame()
        {
            //SimulationSystem.ActiveGame.RegisterPlayer(...);
        }
    }
}
