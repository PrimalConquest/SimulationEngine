using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories
{
    internal static class UnitFactory
    {
        static Dictionary<string, Unit> _parsedUnits = new();

        public static Unit? GetUnit(string unitId)
        {
            if (_parsedUnits.ContainsKey(unitId)) return _parsedUnits[unitId];
            string[]? sRows = ShapeHelper.Parse(unitId);

            if (sRows == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ShapeFactory:GetShape There is no unit with id: {unitId}");
                return null;
            }
            //Unit s = Shape.Parse(sRows);
           // _parsedUnits.Add(unitId, s);
            return null;
        }
    }
}
