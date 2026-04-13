using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events;
using SimulationEngine.Source.Events.Busses;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Interfaces.Events;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Units
{
    public class Unit : IDeepCopyable<Unit>
    {
        Cell _position;

        public Shape Ocupation { get; private set; }
        public Player OwningPlayer { get; private set; }

        public StatSheet Stats { get; private set; }
        public EColor Color { get; private set; }

        Dictionary<Ability, KeyValuePair<EUnitEvent, EventCallback<EventPayload>>> _abilities;

        public bool CanActivate { get { return Stats.GetStat(EStat.Energy) >= GetActivationCost(); } }
        public bool CanMove { get 
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryMove, payload);
                return !payload.Cancelled; 
            } 
        }
        public bool CanFall
        {
            get
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryFall, payload);
                return !payload.Cancelled;
            }
        }

        public bool CanDisplace
        {
            get
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryDisplace, payload);
                return !payload.Cancelled;
            }
        }

        public bool CanBeOverriden
        {
            get
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryOverride, payload);
                return !payload.Cancelled;
            }
        }


        public bool CanBeDrafted
        {
            get
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryDraft, payload);
                return !payload.Cancelled;
            }
        }

        public bool CanRetreat
        {
            get
            {
                EventPayload payload = new();
                UnitEventBus.Raise(EUnitEvent.TryRetreat, payload);
                return !payload.Cancelled;
            }
        }

        public int X { get { return _position.x; } set { _position.x = value; } }
        public int Y { get { return _position.y; } set { _position.y = value; } }
        public Cell Position { get { return _position; } set { _position = value; } }

        public IEventBus<EUnitEvent, EventPayload> UnitEventBus { get; private set; }

        public Unit(Player owningPlayer, EColor color, StatSheet stats, Shape ocupation = default)
        {
            OwningPlayer = owningPlayer;
            Color = color;
            Stats = stats;
            Ocupation = ocupation;
            _abilities = new();

            UnitEventBus = new PriorityEventBus<EUnitEvent, EventPayload>();

            foreach (EUnitEvent type in Enum.GetValues(typeof(EUnitEvent)))
            {
                UnitEventBus.RegisterChannel(type);
            }

            Stats.ListenOnPostStatChange(EStat.Health, new(OnHealthChanged, 2));
            Stats.ListenOnPostStatChange(EStat.Energy, new(OnEnergyChanged, 2));

            UnitEventBus.AddListener(EUnitEvent.TryActivate, new(Activate));

            UnitEventBus.AddListener(EUnitEvent.Move, new(OnMoved));
            UnitEventBus.AddListener(EUnitEvent.Fall, new(OnMoved));
            UnitEventBus.AddListener(EUnitEvent.Displace, new(OnMoved));
        }

        public void Activate(EventPayload payload)
        {
            payload.Cancelled = !CanActivate;
            
            UnitEventBus.Raise(EUnitEvent.Activate, payload);
        }

        public int GetActivationCost()
        {
            int res = 0;
            foreach (KeyValuePair<Ability, KeyValuePair<EUnitEvent, EventCallback<EventPayload>>> ability in _abilities)
            {
                if(ability.Value.Key != EUnitEvent.Activate) continue;
                res += ability.Key.ActivationCost;
            }
            return res;
        }

        private void OnHealthChanged(ValueChangedPayload<int> payload)
        {
            if (payload.Value <= 0)
            {
                UnitEventBus.Raise(EUnitEvent.Die, new());
            }
        }
        private void OnEnergyChanged(ValueChangedPayload<int> payload)
        {
            if (payload.Value <= 0)
            {
                Stats.SetStat(EStat.Energy, 0);
            }
            int max = Stats.GetStat(EStat.MaxEnergy);
            if (payload.Value > max) Stats.SetStat(EStat.Energy, max);
        }
        private void OnMoved(EventPayload payload)
        {
            ValueChangedPayload<Cell>? _payload = payload as ValueChangedPayload<Cell>;
            if (_payload == null) return;
            Position = _payload.Value;
        }

        public void RecieveDamage(Damage damage)
        {
            DamageEventPayload payload = new DamageEventPayload(damage);
            UnitEventBus.Raise(EUnitEvent.RecieveDamage, payload);
            Stats.GrantStat(EStat.Health, -payload.Damage.Value);
        }

        public int GetStat(EStat stat)
        {
            return Stats.GetStat(stat);
        }

        public void GrantAbility(EUnitEvent activation, Ability ability)
        {
            KeyValuePair<EUnitEvent, EventCallback<EventPayload>> hook = new(activation, new(ability.Activate, ability.Priority));
            _abilities.Add(ability, hook);
            UnitEventBus.AddListener(activation, hook.Value);
        }

        public void RemoveAbility(Ability ability)
        {
            _abilities.TryGetValue(ability, out KeyValuePair<EUnitEvent, EventCallback<EventPayload>> hook);
            if(hook.Value == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Unit.RemoveAbility - no ability");
                return;
            }
            UnitEventBus.RemoveListener(hook.Key, hook.Value);
        }

        public Unit DeepCopy()
        {
            Unit copy = new(OwningPlayer, Color, Stats.DeepCopy(), Ocupation);
            
            foreach (KeyValuePair<Ability, KeyValuePair<EUnitEvent, EventCallback<EventPayload>>> ability in _abilities)
            {
                copy.GrantAbility(ability.Value.Key, ability.Key.DeepCopy(copy));
            }

            return copy;
        }
            

        virtual public Unit DeepCopy(Player owningPlayer)
        {
            Unit unit = DeepCopy();
            unit.OwningPlayer = owningPlayer;
            return unit;
        }
    }
}
