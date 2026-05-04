using System.Threading.Tasks;

namespace BattleComunication.Interfaces
{
    public interface ISimulationClient
    {
        Task ReceiveGameSetup(GameStateDTO state);
        Task ReceiveGameState(GameStateDTO state);
        Task ReceiveError(string message);
    }
}
