using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class GrantEnergy : Ability
    {
        public GrantEnergy(Unit owner) : base(owner)
        {
            AddStatDependancy(EStat.Energy);
        }

        public override void Activate(EventPayload payload)
        {
            Dictionary<EStat, uint> stats =  GetActualStats();


        }
    }
}
