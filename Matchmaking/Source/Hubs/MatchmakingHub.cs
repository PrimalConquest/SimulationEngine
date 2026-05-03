using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Matchmaking.Source.Queue;
using Matchmaking.Source.Services;
using MatchmakingComunication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Matchmaking.Source.Hubs
{
    [Authorize]
    public class MatchmakingHub : Hub<IMatchmakingClient>, IMatchmakingHub
    {
        readonly MatchmakingQueue _queue;
        readonly DbWrapperClient  _db;

        public MatchmakingHub(MatchmakingQueue queue, DbWrapperClient db)
        {
            _queue = queue;
            _db    = db;
        }

        // Player calls this after connecting to enter the matchmaking pool.
        public async Task JoinQueue()
        {
            var userId   = Context.User!.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            var userName = Context.User!.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? userId;

            var (stats, err) = await _db.GetStats(userId);
            if (err != null)
            {
                await Clients.Caller.MatchmakingError($"Could not load player stats: {err}");
                return;
            }

            _queue.Enqueue(new QueueEntry
            {
                UserId       = userId,
                UserName     = userName,
                ConnectionId = Context.ConnectionId,
                RankPoints   = stats!.RankPoints,
                QueuedAt     = DateTime.UtcNow,
            });

            await Clients.Caller.QueueJoined(_queue.PositionOf(userId));
        }

        // Player can voluntarily leave the queue.
        public Task LeaveQueue()
        {
            var userId = Context.User!.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            _queue.Dequeue(userId);
            return Clients.Caller.QueueLeft();
        }

        // Auto-remove from queue on disconnect.
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId != null) _queue.Dequeue(userId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
