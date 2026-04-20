using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    public class Player
    {
        static string _commanderStr = "commander";
        public uint Id { get; private set; }
        public Board Board { get; private set; }

        public Unit Commander => SpecialUnits[_commanderStr] ;

        public Dictionary<string, Unit> SpecialUnits { get; private set; }
        public HashSet<Unit> BoardUnits { get; set; }

        public IEventBus<EGameEvent, EventPayload> PlayerEventBus { get; private set; }
        public int CurrentMoves { get; set; }

        public Player(uint id, string commanderId, HashSet<string> officerIds, Cell boardSize)
        {   
            Id = id;
            Board = new Board(boardSize.x, boardSize.y);
            Board.Clear();
            SpecialUnits = new();
            BoardUnits = new();

            PlayerEventBus = new PriorityEventBus<EGameEvent, EventPayload>();
            foreach (EGameEvent type in Enum.GetValues(typeof(EGameEvent)))
                PlayerEventBus.RegisterChannel(type);

            Unit? commander = SimulationSystem.SpawnUnit(commanderId, this);
            if (commander == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"Cannot create commander with id '{commanderId}' for player with id '{Id}'");
                return;
            }
            SpecialUnits.Add(_commanderStr, commander);
            foreach (string offId in officerIds)
            {
                Unit? unit = SimulationSystem.SpawnUnit(offId, this);
                if (unit == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"Cannot create unit with id '{offId}' for player with id '{Id}'");
                    continue;
                }
                SpecialUnits.Add(offId, unit);
            }

            SimulationSystem.SetupBoard(this);
        }

        public void OnTurnStart()
        {
            ValueChangedPayload<int> _payload = new(0, CurrentMoves);
            PlayerEventBus.Raise(EGameEvent.TurnStart, _payload);
            CurrentMoves = _payload.Value;
        }

        public void OnTurnEnd()
        {
            ValueChangedPayload<int> _payload = new(0, CurrentMoves);
            PlayerEventBus.Raise(EGameEvent.TurnEnd, _payload);
            CurrentMoves = _payload.Value;
        }
    }
}