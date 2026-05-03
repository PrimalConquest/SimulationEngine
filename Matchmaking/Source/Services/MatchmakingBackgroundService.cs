using Matchmaking.Source.Hubs;
using Matchmaking.Source.Queue;
using MatchmakingComunication;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SharedUtils.Source.Logging;

namespace Matchmaking.Source.Services
{
    public class MatchmakingBackgroundService : BackgroundService
    {
        // Starting rank-point window for a valid match.
        const int BaseThreshold = 100;
        // Window widens by this many points per minute a player has been waiting.
        const int ExpandPerMinute = 50;

        readonly MatchmakingQueue                          _queue;
        readonly DbWrapperClient                           _db;
        readonly AgonesAllocator                           _agones;
        readonly IHubContext<MatchmakingHub, IMatchmakingClient> _hub;

        public MatchmakingBackgroundService(
            MatchmakingQueue queue,
            DbWrapperClient db,
            AgonesAllocator agones,
            IHubContext<MatchmakingHub, IMatchmakingClient> hub)
        {
            _queue  = queue;
            _db     = db;
            _agones = agones;
            _hub    = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await TryMatchAsync();
                await Task.Delay(2_000, ct);
            }
        }

        // Sweep the rank-sorted queue and greedily match adjacent players whose
        // rank difference falls within the expanding threshold.
        async Task TryMatchAsync()
        {
            var entries = _queue.GetAllSortedByRank();
            if (entries.Count < 2) return;

            var matched = new HashSet<string>();

            for (int i = 0; i < entries.Count - 1; i++)
            {
                var a = entries[i];
                if (matched.Contains(a.UserId)) continue;

                for (int j = i + 1; j < entries.Count; j++)
                {
                    var b = entries[j];
                    if (matched.Contains(b.UserId)) continue;

                    // Use the shorter wait time so neither player alone forces a wide window.
                    double minutesWaited = Math.Min(
                        (DateTime.UtcNow - a.QueuedAt).TotalMinutes,
                        (DateTime.UtcNow - b.QueuedAt).TotalMinutes);

                    double threshold = BaseThreshold + ExpandPerMinute * minutesWaited;

                    if (Math.Abs(a.RankPoints - b.RankPoints) <= threshold)
                    {
                        matched.Add(a.UserId);
                        matched.Add(b.UserId);
                        // Fire-and-forget; errors are logged internally.
                        _ = MatchPairAsync(a, b);
                        break;
                    }
                }
            }
        }

        async Task MatchPairAsync(QueueEntry a, QueueEntry b)
        {
            // Remove both players before any async work so they can't be matched twice.
            _queue.Dequeue(a.UserId);
            _queue.Dequeue(b.UserId);

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"Matched [{a.UserId} ({a.RankPoints} pts)] vs [{b.UserId} ({b.RankPoints} pts)]");

            var (loadoutA, errA) = await _db.GetLoadout(a.UserId);
            var (loadoutB, errB) = await _db.GetLoadout(b.UserId);

            if (errA != null || errB != null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"Loadout fetch failed — A:[{errA}] B:[{errB}]");
                await SendError(a.ConnectionId, b.ConnectionId, "Failed to load player data. Please re-queue.");
                return;
            }

            // Loadouts are serialised as annotations on the GameServer so that the
            // BattleServer can read them from its own Agones metadata at startup.
            var annotations = new Dictionary<string, string>
            {
                ["player1-id"]      = a.UserId,
                ["player1-name"]    = a.UserName,
                ["player1-loadout"] = JsonConvert.SerializeObject(loadoutA),
                ["player2-id"]      = b.UserId,
                ["player2-name"]    = b.UserName,
                ["player2-loadout"] = JsonConvert.SerializeObject(loadoutB),
            };

            var (server, err) = await _agones.AllocateAsync(annotations);
            if (err != null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"Agones allocation failed: [{err}]");
                await SendError(a.ConnectionId, b.ConnectionId, "No server available. Please re-queue.");
                return;
            }

            await _hub.Clients.Client(a.ConnectionId).MatchFound(server!.Ip, server.Port);
            await _hub.Clients.Client(b.ConnectionId).MatchFound(server!.Ip, server.Port);
        }

        Task SendError(string connA, string connB, string msg) => Task.WhenAll(
            _hub.Clients.Client(connA).MatchmakingError(msg),
            _hub.Clients.Client(connB).MatchmakingError(msg));
    }
}
