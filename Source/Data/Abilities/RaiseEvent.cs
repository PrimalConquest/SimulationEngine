using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class RaiseEvent : Ability
    {
        const string EventKey = "Event";

        EUnitEvent _event;

        public RaiseEvent(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null)
            : base(owner, priority, targetingScheme) { }

        public override void Activate(EventPayload payload)
        {
            List<Unit> targets = GetTargets();

            foreach (Unit target in targets)
                target.UnitEventBus.Raise(_event, new EventPayload());
        }

        public override Ability DeepCopy()
        {
            RaiseEvent copy = new(Owner);
            copy._event = _event;
            return copy;
        }

        public override void Extract(JObject spec)
        {
            string? eventStr = spec[EventKey]?.Value<string>();
            if (string.IsNullOrWhiteSpace(eventStr))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"RaiseEvent.Extract - missing or empty '{EventKey}' field");
                return;
            }

            EUnitEvent? evt = UnitEventHelper.ToUnitEvent(eventStr);
            if (evt == null) return;

            _event = evt.Value;
        }
    }
}
