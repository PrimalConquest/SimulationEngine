using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Systems;
using System;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class ValueRefTypeHelper
    {
        public static EValueRefType? ToValueRefType(string str)
        {
            if (Enum.TryParse<EValueRefType>(str, out EValueRefType type))
                return type;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ValueRefTypeHelper.ToValueRefType - Unknown value ref type '{str}'");
            return null;
        }
    }
}
