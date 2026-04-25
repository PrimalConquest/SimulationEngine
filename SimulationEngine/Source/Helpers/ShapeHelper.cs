using Newtonsoft.Json;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Helpers
{
    public static class ShapeHelper
    {
        static string _resource = "Shapes.json";

        public static (int width, int height)? Parse(string id)
        {
            string? json = ResourceSystem.Get(_resource);

            if (json == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeHelper.Parse Could not unpack resource: {_resource}");
                return null;
            }

            Dictionary<string, ShapeData>? shapeMap = JsonConvert.DeserializeObject<Dictionary<string, ShapeData>>(json);

            if (shapeMap == null || !shapeMap.TryGetValue(id, out ShapeData data))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeHelper.Parse There is no shape with id: {id} in resource: {_resource}");
                return null;
            }

            return (data.width, data.height);
        }

        private class ShapeData
        {
            public int width  { get; set; }
            public int height { get; set; }
        }
    }
}
