using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories
{
    public static class ShapeFactory
    {
        static Dictionary<string, Shape> _parsedShapes = new();

        public static Shape GetShape(string shapeId)
        {
            if(_parsedShapes.ContainsKey(shapeId)) return _parsedShapes[shapeId];
            string[]? sRows = ShapeHelper.Parse(shapeId);
            if (sRows == null) throw new ArgumentException($"There is no shape with id: {shapeId}");
            Shape s = Shape.Parse(sRows);
            _parsedShapes.Add(shapeId, s);
            return s;
        }
    }
}
