using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Data.Abilities
{
    internal class GrantAbilities : Ability
    {
        const string AbilitiesKey       = "Abilities";
        const string GlobalAbilitiesKey = "GlobalAbilities";

        List<(EUnitEvent trigger, string abilityId)> _refs       = new();
        List<(EGameEvent trigger, string abilityId)> _globalRefs = new();

        public GrantAbilities(Unit owner, int priority = 5, ITargetingScheme? targetingScheme = null)
            : base(owner, priority, targetingScheme) { }

        public override void Activate(EventPayload payload)
        {
            List<Unit> targets = GetTargets();

            foreach (Unit target in targets)
            {
                foreach ((EUnitEvent trigger, string abilityId) in _refs)
                {
                    Ability? ability = AbilityFactory.GetAbility(abilityId, target);
                    if (ability == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                            $"GrantAbility.Activate - Could not load ability '{abilityId}' for target [{target.Id}]");
                        continue;
                    }
                    target.GrantAbility(trigger, ability);
                }

                foreach ((EGameEvent trigger, string abilityId) in _globalRefs)
                {
                    Ability? ability = AbilityFactory.GetAbility(abilityId, target);
                    if (ability == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                            $"GrantAbility.Activate - Could not load global ability '{abilityId}' for target [{target.Id}]");
                        continue;
                    }
                    target.GrantGlobalAbility(trigger, ability);
                }
            }
        }

        public override Ability DeepCopy()
        {
            GrantAbilities copy = new(Owner);
            copy._refs       = new(_refs);
            copy._globalRefs = new(_globalRefs);
            return copy;
        }

        public override void Extract(JObject spec)
        {
            // Unit-event abilities
            if (spec[AbilitiesKey] is JArray unitArr)
            {
                foreach (JToken token in unitArr)
                {
                    if (token is not JObject obj) continue;
                    AbilityRefData? refData = obj.ToObject<AbilityRefData>();
                    if (refData == null) continue;
                    var parsed = AbilityRefHelper.ParseUnitRef(refData);
                    if (parsed == null) continue;
                    _refs.Add(parsed.Value);
                }
            }
            else
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"GrantAbility.Extract - '{AbilitiesKey}' array missing or invalid, no unit abilities will be granted");
            }

            // Global (game-event) abilities
            if (spec[GlobalAbilitiesKey] is JArray globalArr)
            {
                foreach (JToken token in globalArr)
                {
                    if (token is not JObject obj) continue;
                    AbilityRefData? refData = obj.ToObject<AbilityRefData>();
                    if (refData == null) continue;
                    var parsed = AbilityRefHelper.ParseGameRef(refData);
                    if (parsed == null) continue;
                    _globalRefs.Add(parsed.Value);
                }
            }
        }
    }
}
