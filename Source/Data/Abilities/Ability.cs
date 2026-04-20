using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using System.Collections.Generic;

namespace SimulationEngine.Source.Data.Abilities
{
    public abstract class Ability : IDeepCopyable<Ability>
    {
        Unit _owner;
        ITargetingScheme? _targetingScheme;

        protected Unit Owner { get { return _owner; } }
        public int Priority { get; private set; }

        protected Ability(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null)
        {
            _owner = owner;
            _targetingScheme = targetingScheme;
            Priority = priority;
        }

        protected List<Unit> GetTargets()
        {
            if (_targetingScheme == null) return new();
            return _targetingScheme.GatherTargets(Owner);
        }

        abstract public void Activate(EventPayload payload);
        abstract public void Extract(JObject spec);

        virtual public Ability DeepCopy(Unit owner)
        {
            Ability ability = DeepCopy();
            ability._owner = owner;
            ability._targetingScheme = _targetingScheme;
            ability.Priority = Priority;
            return ability;
        }

        public abstract Ability DeepCopy();
    }
}
