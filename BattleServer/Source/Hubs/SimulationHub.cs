using BattleComunication;
using BattleComunication.Interfaces;
using BattleServer.Source.Mappers;
using BattleServer.Source.Services;
using LoadoutComunication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Factories.Commands;
using SimulationEngine.Source.Factories.Commands.CommandInfos;
using SimulationEngine.Source.Logistic;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BattleServer.Source.Hubs
{
    [Authorize]
    public class SimulationHub : Hub<ISimulationClient>, ISimulationHub
    {
        readonly Game                              _game;
        readonly AgonesService                     _agones;
        readonly DbWrapperClient                   _db;
        readonly ILogger<SimulationHub>            _log;
        readonly ConcurrentDictionary<string, int> _playerIndexByUserId;

        public SimulationHub(
            Game                              game,
            AgonesService                     agones,
            DbWrapperClient                   db,
            ILogger<SimulationHub>            log,
            ConcurrentDictionary<string, int> playerIndexByUserId)
        {
            _game                = game;
            _agones              = agones;
            _db                  = db;
            _log                 = log;
            _playerIndexByUserId = playerIndexByUserId;
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

            int playerIndex = _game.Players.Count - 1;
            _playerIndexByUserId[userId] = playerIndex;
            _log.LogInformation("Registered {Name} ({UserId}) as player index {Index}", playerName, userId, playerIndex);

            if (_game.CanStart)
            {
                _log.LogInformation("Both players connected — starting game");
                _game.Play();
                await Clients.All.ReceiveGameSetup(GameStateMapper.From(_game));
            }

            await base.OnConnectedAsync();
        }

        // ── Command handlers ──────────────────────────────────────────────────────

        public async Task SendMoveCommand(int x, int y, int direction)
        {
            var player = GetActivePlayerForCaller();
            if (player == null) { await Clients.Caller.ReceiveError("Not your turn"); return; }

            var info    = new MoveCommandInfo { Position = new Cell(x, y), Direction = (EDirection)direction };
            var command = CommandFactory.Get(player, info);
            if (command == null || !command.CanExecute())
            {
                await Clients.Caller.ReceiveError("Invalid move command");
                return;
            }

            command.Execute();
            await Clients.All.ReceiveGameState(GameStateMapper.From(_game));
        }

        public async Task SendActivateCommand(string unitKey)
        {
            var player = GetActivePlayerForCaller();
            if (player == null) { await Clients.Caller.ReceiveError("Not your turn"); return; }

            var info    = new ActivateSpecialCommandInfo { UnitId = unitKey };
            var command = CommandFactory.Get(player, info);
            if (command == null || !command.CanExecute())
            {
                await Clients.Caller.ReceiveError("Cannot activate unit");
                return;
            }

            command.Execute();
            await Clients.All.ReceiveGameState(GameStateMapper.From(_game));
        }

        public async Task SendPlaceCommand(string unitKey, int x, int y)
        {
            var player = GetActivePlayerForCaller();
            if (player == null) { await Clients.Caller.ReceiveError("Not your turn"); return; }

            var info    = new PlaceSpecialCommandInfo { UnitId = unitKey, Position = new Cell(x, y) };
            var command = CommandFactory.Get(player, info);
            if (command == null || !command.CanExecute())
            {
                await Clients.Caller.ReceiveError("Cannot place unit there");
                return;
            }

            command.Execute();
            await Clients.All.ReceiveGameState(GameStateMapper.From(_game));
        }

        public async Task SendEndTurn()
        {
            var player = GetActivePlayerForCaller();
            if (player == null) { await Clients.Caller.ReceiveError("Not your turn"); return; }

            var command = CommandFactory.Get(player, new EndTurnCommandInfo());
            if (command == null || !command.CanExecute())
            {
                await Clients.Caller.ReceiveError("Cannot end turn");
                return;
            }

            command.Execute();
            await Clients.All.ReceiveGameState(GameStateMapper.From(_game));
        }

        // ── Match result ──────────────────────────────────────────────────────────

        public async Task ReportMatchResultAsync(string winnerId, string loserId)
        {
            var err = await _db.PostMatchResultAsync(winnerId, loserId);
            if (err != null)
                _log.LogError("Failed to post match result: {Error}", err);

            await _agones.ShutdownAsync();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        Player? GetActivePlayerForCaller()
        {
            var userId = Context.User!.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            if (!_playerIndexByUserId.TryGetValue(userId, out int idx)) return null;
            if (_game.ActivePlayer != idx) return null;
            return _game.Players[idx].Key;
        }
    }
}
