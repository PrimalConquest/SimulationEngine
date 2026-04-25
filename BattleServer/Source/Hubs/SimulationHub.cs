using Microsoft.AspNetCore.SignalR;
using SharedServices.Source.Interfaces;
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

        public void ConnetToGame()
        {
            //SimulationSystem.ActiveGame.RegisterPlayer(...);
        }

        public bool SendCommand(ICommandInfo info)
        {

            //Clients.All.RecieveCommand(info);

            return true;
        }


    }
}
