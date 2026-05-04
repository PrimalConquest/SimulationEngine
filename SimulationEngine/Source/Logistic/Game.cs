using SharedUtils.Source.Logging;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SimulationEngine.Source.Logistic
{
    public class Game
    {
        int _numPlayers;
        bool _initilized;
        Cell _boardSize;
        public int ActivePlayer { get; set; }

        public List<KeyValuePair<Player, string>> Players { get; private set; }
        public Player CurrentPlayer { get { return Players[ActivePlayer].Key; }  }
        public Player OtherPlayer { get { return Players[GetEnemyPlayer(ActivePlayer)].Key; } }

        public Game(int numPlayers, Cell boardSize)
        {
            Players = new();
            ActivePlayer = -1;
            _initilized = false;
            _numPlayers = numPlayers;
            _boardSize = boardSize;
        }

        public KeyValuePair<Player, string>? RegisterPlayer(string playerName, string commanderId, HashSet<string> officerIds)
        {
            if(_initilized == false)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Cannot register players before initializatiuon");
                return null;
            }

            Player player = new(SimulationSystem.NextId(), commanderId, officerIds, _boardSize);

            KeyValuePair<Player, string> p = new(player, playerName);

            Players.Add(p);

            return p;
        }

        public int GetEnemyPlayer(int playerId)
        {
            return (playerId + 1) % Players.Count;
        }

        public void InitGame()
        {
            SimulationSystem.Init(SimulationSystem.RandomInt(), 1, this);
            _initilized = true;
        }

        void StartTurn()
        {
            CurrentPlayer.OnTurnStart();
        }

        public void EndTurn()
        {
            CurrentPlayer.OnTurnEnd();
            ActivePlayer = GetEnemyPlayer(ActivePlayer);
            StartTurn();
        }

        public bool CanStart => Players.Count == _numPlayers;

        public void Play()
        {

            if(!CanStart)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Cannot start game - {Players.Count}/{_numPlayers} players");
                return;
            }

            ActivePlayer = SimulationSystem.RandomInt() % Players.Count;

            StartTurn();
            
        }

    }
}
