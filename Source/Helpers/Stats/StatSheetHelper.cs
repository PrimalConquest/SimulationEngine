using Newtonsoft.Json;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimulationEngine.Source.Helpers.Stats
{
    internal static class StatSheetHelper
    {
        static string _resourceFolder = "SimulationEngine.Resources.StatSheets";

        public static Dictionary<string, ushort>? Parse(string id)
        {
            string _resource = _resourceFolder + "." + id + ".json";
            Assembly assembly = typeof(ShapeHelper).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(_resource))
            {
                if (stream == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper:Parse There is no resource: {_resource}");
                    return null;
                }
                using (StreamReader sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();

                    Dictionary<string,ushort>? statInfo = JsonConvert.DeserializeObject<Dictionary<string, ushort>>(json);

                    if (statInfo == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetHelper:Parse Cannot parse StatSheet in resource: {_resource}");
                        return null;
                    }

                    return statInfo;
                }
            }
        }
    }
}
