using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Units
{
    internal abstract class Unit
    {
        StatSheet _stats;
        public uint Id { get; private set; }
        public EColor Color { get; private set; }

        private IEventBus<string, EventPayload> internalEventBus;

        //Add Activate Ability

        protected Unit(uint id)
        {
            Id = id;
            internalEventBus = new PriorityEventBus<string, EventPayload>();
            //if(color )

            _stats = new();
            /*Stat<ushort> Health = new();
            foreach (EValueType type in Enum.GetValues(typeof(EValueType))) Health.RegisterValue(EValueType.BASE);
            _stats.RegisterAttribute(EStat.Health, Health);*/
        }

        public void Trigger(string signal, EventPayload data)
        {
            internalEventBus.Raise(signal, data);
        }
    }
}
