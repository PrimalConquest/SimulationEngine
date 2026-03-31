using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SimulationEngine.Source.Helpers
{
    public static class ShapeHelper
    {
        public static string[]? Parse(string id)
        {
            Assembly assembly = typeof(ShapeHelper).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("SimulationEngine.Resources.Shapes.json"))
            { 
                using (StreamReader sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();

                    Dictionary<string, string[]>? shapeMap = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

                    if (shapeMap == null) return null;

                    return shapeMap[id];
                }
            }
        }

    }
}
