using Newtonsoft.Json;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Units
{
    internal static class UnitHelper
    {
        static readonly string _resourcePath = "Units.";
        public static UnitData? Parse(string unitId)
        {
            string resource = _resourcePath + unitId + ".json";
            string? json = ResourceSystem.Get(resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitHelper.Parse - Could not load resource: {resource}");
                return null;
            }

            UnitData? data = JsonConvert.DeserializeObject<UnitData>(json);

            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitHelper.Parse - Failed to deserialize AbilityData from: {resource}");
                return null;
            }

            return data;
        }
    }
}
