using System.Threading.Tasks;

namespace BattleComunication.Interfaces
{
    public interface ISimulationHub
    {
        Task SendMoveCommand(int x, int y, int direction);
        Task SendActivateCommand(string unitKey);
        Task SendPlaceCommand(string unitKey, int x, int y);
        Task SendEndTurn();
        Task ReportMatchResultAsync(string winnerId, string loserId);
    }
}
