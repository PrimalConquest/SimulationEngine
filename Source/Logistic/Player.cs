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
        public Dictionary<uint, Unit> Commanders { get; private set; }
        public Dictionary<uint, Unit> Officers { get; private set; }
        public Dictionary<uint, Unit> Units { get; set; }

        public List<uint> PlacedSpecialUnits { get; private set; }

        public IEventBus<EGameEvent, EventPayload> PlayerEventBus { get; private set; }
        public int CurrentMoves { get; set; }

        public Player(uint id, Dictionary<string, Cell> commanderIds, Dictionary<uint, Unit> officers, Board board)
        {
            Id = id;
            Board = board;
            Commanders = new();
            Officers = officers;
            Units = new();
            PlacedSpecialUnits = new();

            PlayerEventBus = new PriorityEventBus<EGameEvent, EventPayload>();

            foreach (EGameEvent type in Enum.GetValues(typeof(EGameEvent)))
            {
                PlayerEventBus.RegisterChannel(type);
            }
            PlayerEventBus.AddListener(EGameEvent.TurnStart, new(ClearRemainingMoves, int.MaxValue));
            PlayerEventBus.AddListener(EGameEvent.TurnEnd, new(ClearRemainingMoves, int.MaxValue));
        }

        public void OnTurnStart()
        {
            PlayerEventBus.Raise(EGameEvent.TurnStart, new());
        }

        public void OnTurnEnd()
        {
            PlayerEventBus.Raise(EGameEvent.TurnEnd, new());
        }
        
        void RefillEnergy(EventPayload payload)
        {
            ValuePayload<int> _payload = new(0);
            PlayerEventBus.Raise(EGameEvent.RefillMoves, _payload);
            CurrentMoves += _payload.Value;
        }

        void ClearRemainingMoves(EventPayload payload)
        {
            CurrentMoves = 0;
        }

    }
}
