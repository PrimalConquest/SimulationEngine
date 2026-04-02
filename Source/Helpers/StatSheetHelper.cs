using Newtonsoft.Json;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimulationEngine.Source.Helpers
{
    internal static class StatSheetHelper
    {
        static string _resourcePath = "StatSheets.";

        public static Dictionary<string, ushort>? Parse(string id)
        {
            string resource = _resourcePath + id + ".json";
            string? json = ResourceSystem.Get(resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper:Parse Could not unpack resource: {resource}");
                return null;
            }

            Dictionary<string, ushort>? statInfo = JsonConvert.DeserializeObject<Dictionary<string, ushort>>(json);

            if (statInfo == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper:Parse Cannot parse StatSheet in resource: {resource}");
                return null;
            }

            return statInfo;
        }
    }
}
