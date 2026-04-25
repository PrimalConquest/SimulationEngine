using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Stats;
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

        public static Dictionary<EStat, int>? Parse(string id)
        {
            string resource = _resourcePath + id + ".json";
            string? json = ResourceSystem.Get(resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper.Parse Could not unpack resource: {resource}");
                return null;
            }

            Dictionary<string, int>? statInfo = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

            if (statInfo == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper.Parse Cannot parse StatSheet in resource: {resource}");
                return null;
            }

            return ConvertKeys(statInfo);
        }

        public static Dictionary<EStat, int>? ConvertKeys(Dictionary<string, int> raw)
        {
            var result = new Dictionary<EStat, int>();
            foreach (var kv in raw)
            {
                EStat? stat = StatHelper.ToStat(kv.Key);
                if (stat.HasValue)
                    result[stat.Value] = kv.Value;
            }
            return result.Count > 0 ? result : null;
        }
    }
}
