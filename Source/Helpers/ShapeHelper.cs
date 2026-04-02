using Newtonsoft.Json;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimulationEngine.Source.Helpers
{
    public static class ShapeHelper
    {
        static string _resource = "SimulationEngine.Resources.Shapes.json";

        public static string[]? Parse(string id)
        {
            Assembly assembly = typeof(ShapeHelper).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(_resource))
            {
                if (stream == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeHelper:Parse There is no resource: {_resource}");
                    return null;
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();

                    Dictionary<string, string[]>? shapeMap = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

                    if (shapeMap == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeHelper:Parse There is no shape with id: {id} in resource: {_resource}");
                        return null;
                    }

                    return shapeMap[id];
                }
            }
        }

    }
}
