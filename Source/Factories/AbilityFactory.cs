using Newtonsoft.Json;
using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;

namespace SimulationEngine.Source.Factories
{
    public class AbilityFactory
    {
        static Dictionary<string, Ability> _parsedAbilities = new();

        public static Ability? GetAbility(string abilityId, Unit owner)
        {
            if (_parsedAbilities.ContainsKey(abilityId))
                return _parsedAbilities[abilityId].DeepCopy(owner);

            AbilityData? data = AbilityHelper.Parse(abilityId);
            if (data == null) return null;

            if (string.IsNullOrWhiteSpace(data.Class))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"AbilityFactory.GetAbility - Ability '{abilityId}' has no 'class' field defined.");
                return null;
            }

            ITargetingScheme? scheme = TargetingSchemeFactory.GetTargetingScheme(data.TargetingId);

            Type? type = Type.GetType("SimulationEngine.Source.Data.Abilities." + data.Class);
            if (type == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"AbilityFactory.GetAbility - Unknown ability class '{data.Class}' in '{abilityId}'");
                return null;
            }

            Ability? ability = (Ability?)Activator.CreateInstance(type, owner, data.Priority, scheme);
            if (ability == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"AbilityFactory.GetAbility - Cannot create ability class '{data.Class}' in '{abilityId}'");
                return null;
            }

            ability.Extract(data.Specific);

            _parsedAbilities.Add(abilityId, ability);
            return ability;
        }
    }
}
