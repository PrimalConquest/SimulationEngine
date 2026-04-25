using Newtonsoft.Json.Linq;
using SharedUtils.Source.Events;
using SharedUtils.Source.Events.Busses;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Stats
{
    public class StatSheet : IDeepCopyable<StatSheet>
    {
        private Dictionary<EStat, int> _stats;

        private IEventBus<EStat, ValuePayload<int>> _onGetStat;
        private IEventBus<EStat, ValuePayload<int>> _onGrantStat;
        private IEventBus<EStat, ValueChangedPayload<int>> _preStatChanged;
        private IEventBus<EStat, ValueChangedPayload<int>> _postStatChanged;

        public StatSheet()
        {
            _stats = new();

            _onGetStat = new PriorityEventBus<EStat, ValuePayload<int>>();
            _onGrantStat = new PriorityEventBus<EStat, ValuePayload<int>>();
            _preStatChanged = new PriorityEventBus<EStat, ValueChangedPayload<int>>();
            _postStatChanged = new PriorityEventBus<EStat, ValueChangedPayload<int>>();
        }

        public StatSheet(Dictionary<EStat, int> info) : this()
        {
            foreach (KeyValuePair<EStat, int> stat in info)
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

        public void RegisterStat(EStat stat, int value)
        {
            _stats.Add(stat, value);
            _onGetStat.RegisterChannel(stat);
            _onGrantStat.RegisterChannel(stat);
            _preStatChanged.RegisterChannel(stat);
            _postStatChanged.RegisterChannel(stat);
        }

        public void SetStat(EStat stat, int value)
        {

            ValueChangedPayload<int> payload = new ValueChangedPayload<int>(value, _stats[stat]);
            _preStatChanged.Raise(stat, payload);
            _stats[stat] = payload.Value;
            _postStatChanged.Raise(stat, payload);
        }
        public void GrantStat(EStat stat, int value)
        {
            ValuePayload<int> payload = new ValuePayload<int>(value);
            SetStat(stat, _stats[stat] + payload.Value);
        }

        public int GetStat(EStat stat)
        {
            _stats.TryGetValue(stat, out var value);
            ValuePayload<int> payload = new ValuePayload<int>(value);
            _onGetStat.Raise(stat, payload);
            return payload.Value;
        }

        public bool ListenOnGetStat(EStat stat, EventCallback<ValuePayload<int>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnGetStat - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _onGetStat.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnGetStat(EStat stat, EventCallback<ValuePayload<int>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _onGetStat.RemoveListener(stat, callback);
            }
            return true;
        }

        public bool ListenOnGrantStat(EStat stat, EventCallback<ValuePayload<int>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnGrantStat - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _onGrantStat.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnGrantStat(EStat stat, EventCallback<ValuePayload<int>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _onGrantStat.RemoveListener(stat, callback);
            }
            return true;
        }

        public bool ListenOnPreStatChange(EStat stat, EventCallback<ValueChangedPayload<int>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnPreStatChange - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _preStatChanged.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnPreStatChange(EStat stat, EventCallback<ValueChangedPayload<int>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _preStatChanged.RemoveListener(stat, callback);
            }
            return true;
        }

        public bool ListenOnPostStatChange(EStat stat, EventCallback<ValueChangedPayload<int>> callback, bool enforceEventCreation = false)
        {
            if (!_stats.ContainsKey(stat))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"StatSheet.ListenOnPostStatChange - Trying to listen on value type that isnt registerd ({stat.ToString()})");
                return false;
            }
            _postStatChanged.AddListener(stat, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnPostStatChange(EStat stat, EventCallback<ValueChangedPayload<int>> callback)
        {
            if (_stats.ContainsKey(stat))
            {
                _postStatChanged.RemoveListener(stat, callback);
            }
            return true;
        }
    }
}
