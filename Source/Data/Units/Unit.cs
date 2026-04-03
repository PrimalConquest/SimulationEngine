using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Units
{
    internal class Unit
    {

        Point _position;

        Shape _ocupation;
        public uint Id { get; private set; }

        public StatSheet Stats { get; private set; }
        public EColor Color { get; private set; }

        public int X { get { return _position.x; } set { _position.x = value; } }
        public int Y { get { return _position.y; } set { _position.y = value; } }
        public Point Position { get { return _position; } set { _position = value; } }

        public IEventBus<EUnitEvent, EventPayload> UnitEventBus { get; private set; }

        public Unit(uint id, StatSheet stats, Shape ocupation = default)
        {
            Id = id;
            Stats = stats;
            _ocupation = ocupation;

            UnitEventBus = new PriorityEventBus<EUnitEvent, EventPayload>();

            foreach (EUnitEvent type in Enum.GetValues(typeof(EUnitEvent)))
            {
                UnitEventBus.RegisterChannel(type);
            }

            Stats.ListenOnValueChange(EStat.Health, new(OnHealthChanged));
        }

        private void OnHealthChanged(ValueChangedPayload<ushort> payload)
        {
            if (payload.Value == 0)
            {
                UnitEventBus.Raise(EUnitEvent.Die, new());
            }
        }

        public uint GetStat(EStat stat)
        {
            return Stats.GetStat(stat);
        }

    }
}
