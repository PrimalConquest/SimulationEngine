using Newtonsoft.Json;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.TargetingScheme
{
    internal static class TargetingSchemeHelper
    {
        static readonly string _resourcePath = "TargetingSchemes.";

        public static TargetingSchemeData? Parse(string abilityId)
        {
            string resource = _resourcePath + abilityId + ".json";
            string? json = ResourceSystem.Get(resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"TargetingSchemeHelper.Parse - Could not load resource: {resource}");
                return null;
            }

            TargetingSchemeData? data = JsonConvert.DeserializeObject<TargetingSchemeData>(json);

            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"TargetingSchemeHelper.Parse - Failed to deserialize TargetingSchemeData from: {resource}");
                return null;
            }

            return data;
        }
    }
}

