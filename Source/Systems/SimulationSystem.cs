using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Systems
{
    internal static class SimulationSystem
    {
        private static Random _rng = new Random();

        private static uint _currentId = 0;

        public static Game ActiveGame { get; private set; }

        public static int Seed 
        {
            get;
            set
            {
                field = value;
                _rng = new Random(field);
            }
        }

        public static int RandomInt() => _rng.Next();
        public static uint NextId() => ++_currentId;

        public static void Init(int seed, uint currentId, Game game)
        {
            Seed = seed;
            _currentId = currentId;
            ActiveGame = game;
        }

        static SimulationSystem()
        {
            Seed = new Random().Next();
            ActiveGame = new(new());
        }

        public static KeyValuePair<uint, Unit>? SpawnUnit(string unitId, Player owner)
        {
            Unit? unit = UnitFactory.GetUnit(unitId, owner);
            if (unit == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"SimulationSystem.SpawnUnit Cannot spawn with id: {unitId} for player[{owner.Id}]");
                return null;
            }
            uint simId = SimulationSystem.NextId();
            return new KeyValuePair<uint, Unit>(simId, unit);
        }
    }
}
