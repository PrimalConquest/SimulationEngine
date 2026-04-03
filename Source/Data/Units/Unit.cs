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
        StatSheet _stats;
        public uint Id { get; private set; }
        public EColor Color { get; private set; }
        
        Point _position;

        Shape _ocupation;

        protected IEventBus<EUnitEvent, EventPayload> _unitEventBus;

        //Add Activate Ability

        public int X { get { return _position.X; } set { _position.X = value; } }
        public int Y { get { return _position.Y; } set { _position.Y = value; } }
        public Point Position { get { return _position; } set { _position = value; } }

        public Unit(uint id, StatSheet stats, Shape ocupation = default)
        {
            Id = id;
            _stats = stats;
            _ocupation = ocupation;

            _unitEventBus = new PriorityEventBus<EUnitEvent, EventPayload>();

            foreach (EUnitEvent type in Enum.GetValues(typeof(EUnitEvent)))
            {
                _unitEventBus.RegisterChannel(type);
            }

            _unitEventBus.AddListener(EUnitEvent.GetStat, new(OnGetStat));

            _stats.ListenOnValueChange(EStat.Health, new(OnHealthChanged));
        }

        private void OnHealthChanged(ValueChangedPayload<ushort> payload)
        {
            if (payload.Value == 0)
            {
                Trigger(EUnitEvent.Die, new());
            }
        }

        private void OnGetStat(EventPayload payload)
        {
            StatPayload? statPayload = payload as StatPayload;
            if(statPayload == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, "Unit.OnGetStat - payload is not of type StatPayload");
                return;
            }

             statPayload.Value = _stats.GetStat(statPayload.Stat);
        }

        public void Trigger(EUnitEvent signal, EventPayload data)
        {
            _unitEventBus.Raise(signal, data);
        }
    }
}
