using BattleComunication.Interfaces;
using BattleServer.Source.Services;
using LoadoutComunication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BattleServer.Source.Hubs
{
    [Authorize]
    public class SimulationHub : Hub<ISimulationClient>, ISimulationHub
    {
        readonly Game            _game;
        readonly AgonesService   _agones;
        readonly DbWrapperClient _db;
        readonly ILogger<SimulationHub> _log;

        public SimulationHub(Game game, AgonesService agones, DbWrapperClient db, ILogger<SimulationHub> log)
        {
            _game   = game;
            _agones = agones;
            _db     = db;
            _log    = log;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

            var json = await _agones.GetGameServerJsonAsync();
            if (json == null)
            {
                _log.LogError("Agones sidecar unreachable — cannot register player {UserId}", userId);
                return;
            }

            var annotations = JObject.Parse(json)["metadata"]!["annotations"]!;

            var player1Id      = annotations["player1-id"]!.ToString();
            var player1Name    = annotations["player1-name"]!.ToString();
            var player1Loadout = JsonConvert.DeserializeObject<LoadoutDTO>(annotations["player1-loadout"]!.ToString())!;

            var player2Id      = annotations["player2-id"]!.ToString();
            var player2Name    = annotations["player2-name"]!.ToString();
            var player2Loadout = JsonConvert.DeserializeObject<LoadoutDTO>(annotations["player2-loadout"]!.ToString())!;

            string playerName;
            LoadoutDTO loadout;

            if (userId == player1Id)        { playerName = player1Name; loadout = player1Loadout; }
            else if (userId == player2Id)   { playerName = player2Name; loadout = player2Loadout; }
            else
            {
                _log.LogWarning("Unexpected player connected: {UserId}", userId);
                return;
            }

            _game.RegisterPlayer(playerName, loadout.CommanderId, new HashSet<string>(loadout.OfficerIds));
            _log.LogInformation("Registered {Name} ({UserId})", playerName, userId);

            StartGame();

            await base.OnConnectedAsync();
        }

        public bool SendCommand(ICommandInfo info)
        {
            // TODO: route command to the correct player via _game and check for game over.
            // When the SimulationEngine raises EGameEvent.GameOver, call:
            //   await ReportMatchResultAsync(winnerId, loserId);
            return true;
        }

        // Called once the SimulationEngine determines the match outcome.
        // Updates RankPoints in DBWrapper then shuts down this GameServer pod.
        public async Task ReportMatchResultAsync(string winnerId, string loserId)
        {
            var err = await _db.PostMatchResultAsync(winnerId, loserId);
            if (err != null)
                _log.LogError("Failed to post match result: {Error}", err);

            await _agones.ShutdownAsync();
        }

        public void StartGame()
        {
            if (!_game.CanStart)
            {
                _log.LogInformation("Still cannot start game...");
                return;
            }

            _log.LogInformation("Both players connected — starting game");
            _game.Play();
        }
    }
}
