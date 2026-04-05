using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Abilities
{
    internal abstract class Ability
    {
        Unit _owner;
        //HashSet<EStat> _scalingStats;
        ITargetingScheme? _targetingScheme;
        //Dictionary<EStat, uint>? _presetStats;
        protected Unit Owner { get { return _owner; } }

        public int ActivationCost { get; set; }
        public int Priority { get; private set; }
        protected Ability(Unit owner, int priority = 5,ITargetingScheme? targetingScheme = null)
        {
            _owner = owner;
            //_scalingStats = new();
            _targetingScheme = targetingScheme;
        }

        /*public void InjectPresetStats(Dictionary<EStat, uint> presetStats)
        {
            _presetStats = presetStats;
        }

        protected void AddStatDependancy(EStat stat)
        {
            _scalingStats.Add(stat);
        }

        protected Dictionary<EStat, uint> GetActualStats(List<Unit> targets)
        {
            Dictionary<EStat, uint> stats = (_presetStats==null) ? new() : new(_presetStats);
            foreach (EStat stat in _scalingStats)
            {
                if (stats.ContainsKey(stat)) continue;
                stats.Add(stat,Owner.GetStat(stat));
            }
            return stats;
        }*/

        protected List<Unit> GetTargets()
        {
            if(_targetingScheme == null) return new();

            return _targetingScheme.GatherTargets(Owner);
        }

        abstract public void Activate(EventPayload payload);
        abstract public void Extract(string json);
    }
}
