using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class RequireActivationReady : Ability
    {
        public RequireActivationReady(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null)
            : base(owner, priority, targetingScheme) { }

        public override void Activate(EventPayload payload)
        {
            payload.Cancelled = !Owner.CanActivate;
        }

        public override Ability DeepCopy() => new RequireActivationReady(Owner);

        public override void Extract(JObject spec) { }
    }
}
