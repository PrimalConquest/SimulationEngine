using SharedUtils.Source.Logging;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Helpers.Abilities
{
    internal static class AbilityRefHelper
    {
        const string TriggerKey   = "Trigger";
        const string AbilityIdKey = "AbilityId";

        public static (EUnitEvent trigger, string abilityId)? ParseUnitRef(AbilityRefData data)
        {
            if (string.IsNullOrWhiteSpace(data.AbilityId))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"AbilityRefHelper.ParseUnitRef - missing or empty '{AbilityIdKey}'");
                return null;
            }

            EUnitEvent? trigger = UnitEventHelper.ToUnitEvent(data.Trigger);
            if (trigger == null) return null;

            return (trigger.Value, data.AbilityId);
        }
        public static (EGameEvent trigger, string abilityId)? ParseGameRef(AbilityRefData data)
        {
            if (string.IsNullOrWhiteSpace(data.AbilityId))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"AbilityRefHelper.ParseGameRef - missing or empty '{AbilityIdKey}'");
                return null;
            }

            EGameEvent? trigger = GameEventHelper.ToGameEvent(data.Trigger);
            if (trigger == null) return null;

            return (trigger.Value, data.AbilityId);
        }
    }
}
