using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class GrantMoves : Ability
    {

        static string Amount = "Amount";

        int _amount;

        public GrantMoves(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null) : base(owner, priority, targetingScheme)
        {
        }

        public override void Activate(EventPayload payload)
        {
            ValueChangedPayload<int>? _payload = payload as ValueChangedPayload<int>;
            if (_payload == null) return;
            _payload.Value += _amount;

        }

        public override Ability DeepCopy()
        {
            GrantMoves ability = new(Owner);

            ability._amount = _amount;

            return ability;

        }

        public override void Extract(JObject spec)
        {
            foreach (var prop in spec.Properties())
            {
                if(prop.Name == Amount)
                {
                    _amount = prop.Value.Value<int>();
                    break;
                }
            }
        }
    }
}
