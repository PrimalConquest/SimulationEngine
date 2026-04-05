using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class GrantStat : Ability
    {
        public GrantStat(Unit owner) : base(owner)
        {
        }

        public override void Activate(EventPayload payload)
        {
            List<Unit> targets = GetTargets();

            if (targets.Count == 0) return;

        }
        public override void Extract(string json)
        {
            //throw new NotImplementedException();
        }
    }
}
