using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    public class Player
    {
        public uint Id { get; private set; }
        public Board Board { get; private set; }

        public uint CommanderId { get; private set; }
        public Dictionary<uint, Unit> SpecialUnits { get; private set; }
        public Dictionary<uint, Unit> BoardUnits { get; set; }


        public IEventBus<EGameEvent, EventPayload> PlayerEventBus { get; private set; }
        public int CurrentMoves { get; set; }

        public Player(uint id, Dictionary<string, Cell> commanderIds, Dictionary<uint, Unit> officers, Board board)
        {
            Id = id;
            Board = board;
            SpecialUnits = new();
            BoardUnits = new();

            PlayerEventBus = new PriorityEventBus<EGameEvent, EventPayload>();

            foreach (EGameEvent type in Enum.GetValues(typeof(EGameEvent)))
            {
                PlayerEventBus.RegisterChannel(type);
            }
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
