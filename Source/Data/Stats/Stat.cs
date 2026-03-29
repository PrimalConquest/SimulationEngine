using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Data.EventPayloads;
using SimulationEngine.Source.Data.Stats.Enums;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Busses.Interfaces;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Stats
{
    internal class Stat<T>
    {
        private Dictionary<EValueType, T?> _values;
        private IEventBus<EValueType> _onValueChanged;
        private IEventBus<EValueType> _onGetValue;

        public Stat()
        {
            _values = new();
            _onValueChanged = new PriorityEventBus<EValueType>();
            _onGetValue = new PriorityEventBus<EValueType>();
        }
        public Stat(IEventBus<EValueType> onValueChangedBus, IEventBus<EValueType> onGetValue)
        {
            _values = new();
            _onValueChanged = onValueChangedBus;
            _onGetValue = onGetValue;
        }

        public Stat(T baseValue) : this()
        {
            RegisterValue(EValueType.BASE, baseValue);
        }

        public Stat(T baseValue, T currentValue) : this(baseValue)
        {
            RegisterValue(EValueType.CURRENT, currentValue);
        }

        public Stat(T baseValue, T currentValue, T maxValue) : this(baseValue, currentValue)
        {
            RegisterValue(EValueType.MAX, maxValue);
        }

        public Stat(T baseValue, T currentValue, T maxValue, T minValue) : this(baseValue, currentValue, maxValue)
        {
            RegisterValue(EValueType.MIN, minValue);
        }

        public void RegisterValue(EValueType type, T? value = default)
        {
            SetValue(type, value);
            _onValueChanged.RegisterChannel(type);
        }

        public void SetValue(EValueType type, T? value)
        {
            _values.Add(type, value);
        }

        public T? GetValue(EValueType type)
        {
            _values.TryGetValue(type, out T? value);

            ValuePayload<T?> payload = new ValuePayload<T?>(value);

            _onGetValue.Raise(type, payload);

            return payload.Value;
        }

        public bool ListenOnValueChange(EValueType type, EventCallback<EventPayload> callback, int priority = 0, bool enforceEventCreation = false)
        {
            if(!_values.ContainsKey(type)) return false;

            _onValueChanged.AddListener(type, callback, priority, enforceEventCreation);

            return true;
        }

        public bool StopListenOnValueChange(EValueType type, EventCallback<EventPayload> callback)
        {
            if (_values.ContainsKey(type))
            { 
                _onValueChanged.RemoveListener(type, callback);
            }
            return true;
        }

        public bool ClearListenersOnValueChange(EValueType type)
        {
            if (_values.ContainsKey(type))
            {
                _onValueChanged.ClearChannel(type);
            }
            return true;
        }

    }
}
