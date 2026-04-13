using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class GameEventHelper
    {
        public static EGameEvent? ToGameEvent(string str)
        {
            if (Enum.TryParse<EGameEvent>(str, out EGameEvent color))
                return color;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"GameEventHelper.ToGameEvent - Unknown game event key '{str}'");
            return null;
        }
    }
}
