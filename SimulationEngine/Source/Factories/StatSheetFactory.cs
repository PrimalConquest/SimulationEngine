using SharedUtils.Source.Logging;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories
{
    public class StatSheetFactory
    {
        static Dictionary<string, StatSheet> _parsedShapes = new();

        public static StatSheet? GetSheet(string sheetId)
        {
            if (_parsedShapes.ContainsKey(sheetId))
            {
                return  _parsedShapes[sheetId].DeepCopy();
            }
            Dictionary<EStat, int>? StatInfo = StatSheetHelper.Parse(sheetId);

            if (StatInfo == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatSheetFactory.GetSheet There is no stat sheet with id: {sheetId}");
                return null;
            }
            
            return new(StatInfo);
        }
    }
}
