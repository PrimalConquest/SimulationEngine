using SimulationEngine.Source.Data.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    internal class DamageEventPayload : EventPayload
    {
        public Damage Damage { get; set; }

        public DamageEventPayload(Damage damage)
        {
            Damage = damage;
        }
    }
}
