using SimulationEngine.Source.Events.Busses;
using System;
using System.Collections.Generic;
using System.Text;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Enums.Stats;

namespace SimulationEngine.Source.Data.Stats
{
    internal class StatSheet
    {
        private Dictionary<EStat, Stat<ushort>> _attributes;

        public StatSheet()
        {
            _attributes = new();
        }
        /*public StatSheet(IEventBus<EStat> onValueChangedBus)
        {
            _attributes = new();
            _onStatChanged = onValueChangedBus;
        }*/

        public void RegisterStat(EStat stat, Stat<ushort> attribute)
        {
            _attributes.Add(stat, attribute);
        }

        public Stat<ushort> GetStat(EStat stat)
        {
            _attributes.TryGetValue(stat, out var attribute);
            return attribute;
        } 
    }
}
