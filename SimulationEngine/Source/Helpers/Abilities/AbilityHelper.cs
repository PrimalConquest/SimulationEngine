using Newtonsoft.Json;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Abilities
{
    internal static class AbilityHelper
    {
        static readonly string _resourcePath = "Abilities.";

        public static AbilityData? Parse(string abilityId)
        {
            string resource = _resourcePath + abilityId + ".json";
            string? json = ResourceSystem.Get(resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"AbilityHelper.Parse - Could not load resource: {resource}");
                return null;
            }

            AbilityData? data = JsonConvert.DeserializeObject<AbilityData>(json);

            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"AbilityHelper.Parse - Failed to deserialize AbilityData from: {resource}");
                return null;
            }

            return data;
        }
    }
}
