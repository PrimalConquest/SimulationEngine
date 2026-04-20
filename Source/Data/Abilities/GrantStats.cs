using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class GrantStats : Ability
    {
        Dictionary<EStat, IValueRef> _stats;

        public GrantStats(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null) : base(owner, priority, targetingScheme) 
        {
            _stats = new();
        }

        public override void Activate(EventPayload payload)
        {
            List<Unit> targets = GetTargets();

            foreach (Unit target in targets)
            {
                foreach(KeyValuePair<EStat, IValueRef> stat in _stats)
                {
                    int value = stat.Value.Resolve(stat.Key, Owner, target);
                    target.Stats.GrantStat(stat.Key, value);
                }
            }
        }

        public override Ability DeepCopy()
        {
            GrantStats copy = new(Owner);
            copy._stats = new(_stats);
            return copy;
        }


        public override void Extract(JObject spec)
        {
            foreach (var prop in spec.Properties())
            {
                EStat? s = StatHelper.ToStat(prop.Name);
                if (s == null) continue;

                IValueRef? valueRef = ValueRefHelper.Parse(prop.Value);
                if (valueRef == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"GrantStat.Extract - failed to parse ValueRef for stat '{prop.Name}'");
                    continue;
                }

                _stats.Add(s.Value, valueRef);
            }
        }
    }
}
