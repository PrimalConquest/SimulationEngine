using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Abilities
{
    internal abstract class Ability
    {
        Unit _owner;
        HashSet<EStat> _dependingStats;
        Dictionary<EStat, uint>? _presetStats;
        protected Unit Owner { get { return _owner; } }
        protected Ability(Unit owner)
        {
            _owner = owner;
            _dependingStats = new();
        }

        public void InjectPresetStats(Dictionary<EStat, uint> presetStats)
        {
            _presetStats = presetStats;
        }

        protected void AddStatDependancy(EStat stat)
        {
            _dependingStats.Add(stat);
        }

        protected Dictionary<EStat, uint> GetActualStats()
        {
            Dictionary<EStat, uint> stats = (_presetStats==null) ? new() : new(_presetStats);
            foreach (EStat stat in _dependingStats)
            {
                if (stats.ContainsKey(stat)) continue;
                stats.Add(stat,Owner.GetStat(stat));
            }
            return stats;
        }

        abstract public void Activate(EventPayload payload);
    }
}
