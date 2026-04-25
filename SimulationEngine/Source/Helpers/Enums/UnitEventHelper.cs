using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Enums
{
    internal static class UnitEventHelper
    {
        public static EUnitEvent? ToUnitEvent(string str)
        {
            if (Enum.TryParse<EUnitEvent>(str, out EUnitEvent color))
                return color;

            LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitEventHelper.ToUnitEvent - Unknown unit event key '{str}'");
            return null;
        }
    }
}
