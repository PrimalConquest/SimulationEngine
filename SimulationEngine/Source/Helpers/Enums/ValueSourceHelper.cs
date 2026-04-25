using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Systems;
using System;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class ValueSourceHelper
    {
        public static EValueSource? ToValueSource(string str)
        {
            if (Enum.TryParse<EValueSource>(str, out EValueSource source))
                return source;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ValueSourceHelper.ToValueSource - Unknown value source '{str}'");
            return null;
        }
    }
}
