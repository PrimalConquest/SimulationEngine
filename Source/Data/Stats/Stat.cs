using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Systems;
using SimulationEngine.Source.Enums.Logging;

namespace SimulationEngine.Source.Data.Stats
{
    internal class Stat
    {
        private Dictionary<EValueType, int> _values;
        private IEventBus<EValueType, StatPayload> _onGetStat;
        private IEventBus<EValueType, StatChangedPayload> _preStatChanged;
        private IEventBus<EValueType, StatChangedPayload> _postStatChanged;

        public Stat()
        {
            _values = new();
            _onGetStat = new PriorityEventBus<EValueType, StatPayload>();
            _preStatChanged = new PriorityEventBus<EValueType, StatChangedPayload>();
            _postStatChanged = new PriorityEventBus<EValueType, StatChangedPayload>();
        }

        public Stat(int currentValue = 0) : this()
        {
            RegisterValue(EValueType.Current, currentValue);
        }

        public Stat( int currentValue, int minValue) : this(currentValue)
        {
            RegisterValue(EValueType.Min, minValue);
        }

        public Stat(int currentValue, int minValue, int maxValue) : this(currentValue, minValue)
        {
            RegisterValue(EValueType.Max, maxValue);
        }

        public void RegisterValue(EValueType type, int value = 0)
        {
            SetValue(type, value);
            _onGetStat.RegisterChannel(type);
            _preStatChanged.RegisterChannel(type);
            _postStatChanged.RegisterChannel(type);
        }

        private void SetCurrent(int value)
        {
            int? min = GetValue(EValueType.Min);
            int? max = GetValue(EValueType.Max);
            if (min == null)
            {
                if(max == null)
                {

                }
                else
                {

                }
                //int actual = max.Value;
                //Math.Clamp(value, value, max)
                //Math.Clamp(value, , GetValue(EValueType.Max));
            }
        }

        private void SetMin(int value)
        {

        }

        private void SetMax(int value)
        {

        }

        public void SetValue(EValueType type, int value)
        {
            int? old = GetValue(type);
            int actual;
            if(old == null)
            {
                _values.Add(type, value);
                actual = value;
            }
            else
            {
                actual = old.Value;
            }
            StatChangedPayload payload = new(type, value, actual);
            _preStatChanged.Raise(type, payload);
            int final = payload.Value;

            switch (type)
            {
                case EValueType.Current:
                    {
                        
                        break;
                    }
                case EValueType.Min:
                    {
                        if (final > GetValue(EValueType.Max))
                        {
                            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, "Stat.SetValue - Trying to set min value greather than max value");
                            return;
                        }
                        break;
                    }
                case EValueType.Max:
                    {
                        if (final < GetValue(EValueType.Min))
                        {
                            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, "Stat.SetValue - Trying to set max value lesser than min value");
                            return;
                        }
                        break;
                    }
            }

            _values.Add(type, final);
            //payload.Value = GetValue(type);
            _preStatChanged.Raise(type, payload);
        }

        public int? GetValue(EValueType type)
        {
            if(_values.ContainsKey(type))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, "Stat.GetValue - Trying to get valueType taht isnt registered");
                return null;
            }
            _values.TryGetValue(type, out int value);

            StatPayload payload = new StatPayload(type,value);

            _onGetStat.Raise(type, payload);

            return payload.Value;
        }

        public bool ListenOnValueChange(EValueType type, EventCallback<ValuePayload<int>> callback, bool enforceEventCreation = false)
        {
            if (!_values.ContainsKey(type))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"Stat:ListenOnvalueChange - Trying to listen on value type thaint isnint registerd ({type.ToString()})");
                return false;
            }
            //_onValueChanged.AddListener(type, callback, enforceEventCreation);

            return true;
        }

        public bool StopListenOnValueChange(EValueType type, EventCallback<ValuePayload<int>> callback)
        {
            if (_values.ContainsKey(type))
            { 
                //_onValueChanged.RemoveListener(type, callback);
            }
            return true;
        }

        public bool ClearListenersOnValueChange(EValueType type)
        {
            if (_values.ContainsKey(type))
            {
                //_onValueChanged.ClearChannel(type);
            }
            return true;
        }

    }
}
