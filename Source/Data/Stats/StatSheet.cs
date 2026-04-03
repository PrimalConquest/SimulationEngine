using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Stats
{
    public class StatSheet
    {
        private Dictionary<EStat, ushort> _stats;

        private IEventBus<EStat, ValuePayload<ushort>> _onGetValue;
        //make it into value changed PAYLOAD-----------------------------------------------------------------------------------------
        private IEventBus<EStat, ValueChangedPayload<ushort>> _onValueChanged;

        public StatSheet()
        {
            _stats = new();
            _onValueChanged = new PriorityEventBus<EStat, ValueChangedPayload<ushort>>();
            _onGetValue = new PriorityEventBus<EStat, ValuePayload<ushort>>();
        }

        public StatSheet(Dictionary<EStat, ushort> info) : this()
        {
            foreach (KeyValuePair<EStat, ushort> stat in info)
            {
                RegisterStat(stat.Key, stat.Value);
            }
        }
        public StatSheet DeepCopy()
        {
            StatSheet copy = new StatSheet();

            foreach (var kv in _stats)
            {
                copy.RegisterStat(kv.Key, kv.Value);
            }
            return copy;
        }

        public void RegisterStat(EStat stat, ushort value)
        {
            _stats.Add(stat, value);
            _onGetValue.RegisterChannel(stat);
            _onValueChanged.RegisterChannel(stat);
        }

        public void SetStat(EStat stat, ushort value)
        {

            ValueChangedPayload<ushort> payload = new ValueChangedPayload<ushort>(value, _stats[stat]);
            _stats[stat] = value;
            _onValueChanged.Raise(stat, payload);
        }

        public ushort GetStat(EStat stat)
        {
            _stats.TryGetValue(stat, out var value);
            ValuePayload<ushort> payload = new ValuePayload<ushort>(value);
            _onGetValue.Raise(stat, payload);
            return payload.Value;
        }

        public bool ListenOnGetValue(EStat stat, EventCallback<ValuePayload<ushort>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnGetValue - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _onGetValue.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnGetValue(EStat stat, EventCallback<ValuePayload<ushort>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _onGetValue.RemoveListener(stat, callback);
            }
            return true;
        }

        public bool ListenOnValueChange(EStat stat, EventCallback<ValueChangedPayload<ushort>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnvalueChange - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _onValueChanged.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnValueChange(EStat stat, EventCallback<ValueChangedPayload<ushort>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _onValueChanged.RemoveListener(stat, callback);
            }
            return true;
        }
    }
}
