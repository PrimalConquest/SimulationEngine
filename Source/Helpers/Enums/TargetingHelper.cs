using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class TargetingHelper
    {
        public static ETargeting? ToTargeting(string str)
        {
            if (Enum.TryParse<ETargeting>(str, out ETargeting color))
                return color;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"TargetingHelper.ToTargeting - Unknown target type '{str}'");
            return null;
        }
    }
}
