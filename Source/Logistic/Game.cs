using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    public class Game
    {

        public List<Player> Players { get; private set; }
        public int ActivePlayer { get; private set; }

        public Game(Cell boardSize)
        {
            Players = new();
        }

        void RegisterPlayer(Player player)
        {
            Players.Add(player);
            
        }

        void InitGame()
        {
            SimulationSystem.Init(SimulationSystem.RandomInt(), 1, this);
            ActivePlayer = SimulationSystem.RandomInt() % Players.Count;
        }

        KeyValuePair<uint, Unit>? SpawnUnit(string unitId, Player owner)
        {
            Unit? unit = UnitFactory.GetUnit(unitId, owner);
            if (unit == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"Game.SpawnUnit Cannot spawn with id: {unitId} for player[{owner.Id}]");
                return null;
            }
            uint simId = SimulationSystem.NextId();
            return new KeyValuePair<uint, Unit>(simId, unit);
        }
    }
}
