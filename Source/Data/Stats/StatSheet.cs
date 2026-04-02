using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Stats
{
    internal class StatSheet
    {
        private Dictionary<string, ushort> _stats;

        private IEventBus<string, ValuePayload<ushort>> _onGetValue;
        //make it into value changed PAYLOAD-----------------------------------------------------------------------------------------
        private IEventBus<string, ValuePayload<ushort>> _onValueChanged;

        public StatSheet()
        {
            _stats = new();
            _onValueChanged = new PriorityEventBus<string, ValuePayload<ushort>>();
            _onGetValue = new PriorityEventBus<string, ValuePayload<ushort>>();
        }

        public void RegisterStat(string stat, ushort value)
        {
            _stats.Add(stat, value);
            _onGetValue.RegisterChannel(stat);
            _onValueChanged.RegisterChannel(stat);
        }

        public void SetStat(string stat, ushort value)
        {
            _stats[stat] = value;
            ValuePayload<ushort> payload = new ValuePayload<ushort>(value);
            _onValueChanged.Raise(stat, payload);
        }

        public ushort GetStat(string stat)
        {
            _stats.TryGetValue(stat, out var value);
            ValuePayload<ushort> payload = new ValuePayload<ushort>(value);
            _onGetValue.Raise(stat, payload);
            return payload.Value;
        } 
    }
}
