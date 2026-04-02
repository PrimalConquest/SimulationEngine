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

        static string _resource = "Shapes.json";

        public static string[]? Parse(string id)
        {
            
            string? json = ResourceSystem.Get(_resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeHelper:Parse Could not unpack resource: {_resource}");
                return null;
            }

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
