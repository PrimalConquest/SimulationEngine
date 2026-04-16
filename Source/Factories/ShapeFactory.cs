using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Factories
{
    public static class ShapeFactory
    {
        static Dictionary<string, Shape> _parsedShapes = new();

        public static Shape GetShape(string shapeId)
        {
            if (_parsedShapes.ContainsKey(shapeId))
                return _parsedShapes[shapeId];

            (int width, int height)? data = ShapeHelper.Parse(shapeId);

            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeFactory:GetShape There is no shape with id: {shapeId}");
                return new Shape();
            }

            Shape s = new Shape((uint)data.Value.width, (uint)data.Value.height);
            _parsedShapes.Add(shapeId, s);
            return s;
        }
    }
}
