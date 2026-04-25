using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class ColorHelper
    {
        public static EColor? ToColor(string str)
        {
            if (Enum.TryParse<EColor>(str, out EColor color))
                return color;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"ColorHelper.ToColor - Unknown color key '{str}'");
            return null;
        }
    }
}
