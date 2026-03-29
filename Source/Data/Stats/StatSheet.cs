using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Data.Stats.Enums;
using SimulationEngine.Source.Events.Busses.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Stats
{
    internal class StatSheet
    {
        private Dictionary<EStat, Stat<ushort>> _attributes;
        private IEventBus<EStat> _onStatChanged;

        public StatSheet()
        {
            _attributes = new();
            _onStatChanged = new PriorityEventBus<EStat>();
        }
        public StatSheet(IEventBus<EStat> onValueChangedBus)
        {
            _attributes = new();
            _onStatChanged = onValueChangedBus;
        }

        public void RegisterAttribute(EStat stat, Stat<ushort> attribute)
        {
            _attributes.Add(stat, attribute);
        }

        public Stat<ushort> GetAttribute(EStat stat)
        {
            _attributes.TryGetValue(stat, out var attribute);
            return attribute;
        } 
    }
}
