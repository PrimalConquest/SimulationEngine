using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SimulationEngine.Source.Logistic
{
    public class Game
    {

        public List<Player> Players { get; private set; }
        private int ActivePlayer {get; set; }

        public Player CurrentPlayer { get { return Players[ActivePlayer]; }  }
        public Player OtherPlayer { get { return Players[GetEnemyPlayer(ActivePlayer)]; } }

        public Game(Cell boardSize)
        {
            Players = new();
        }

        void RegisterPlayer(Player player)
        {
            Players.Add(player);
            
        }

        public int GetEnemyPlayer(int playerId)
        {
            return (playerId + 1) % Players.Count;
        }

        void InitGame()
        {
            SimulationSystem.Init(SimulationSystem.RandomInt(), 1, this);
            ActivePlayer = SimulationSystem.RandomInt() % Players.Count;
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

        public void Play()
        {
           
            StartTurn();
            
        }

        public void Advance(ICommand command)
        {
            
        }

    }
}
