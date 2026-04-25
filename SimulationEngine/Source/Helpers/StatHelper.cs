using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers
{
    internal static class StatHelper
    {
        public static EStat? ToStat(string key)
        {
            if (Enum.TryParse<EStat>(key, out EStat stat))
                return stat;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"StatHelper.ToStat - Unknown stat key '{key}'");
            return null;
        }
    }
}
