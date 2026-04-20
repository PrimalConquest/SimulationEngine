using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Helpers.Abilities
{
    internal static class AbilityRefHelper
    {
        const string TriggerKey   = "Trigger";
        const string AbilityIdKey = "AbilityId";

        /// <summary>
        /// Parses an <see cref="AbilityRefData"/> (already deserialized from JSON) into
        /// a typed unit-event + ability-id pair.
        /// </summary>
        public static (EUnitEvent trigger, string abilityId)? ParseUnitRef(AbilityRefData data)
        {
            if (string.IsNullOrWhiteSpace(data.AbilityId))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error,
                    $"AbilityRefHelper.ParseUnitRef - missing or empty '{AbilityIdKey}'");
                return null;
            }

            EUnitEvent? trigger = UnitEventHelper.ToUnitEvent(data.Trigger);
            if (trigger == null) return null;

            return (trigger.Value, data.AbilityId);
        }

        /// <summary>
        /// Parses an <see cref="AbilityRefData"/> into a game-event + ability-id pair
        /// (used for global abilities that listen on <see cref="EGameEvent"/>).
        /// </summary>
        public static (EGameEvent trigger, string abilityId)? ParseGameRef(AbilityRefData data)
        {
            if (string.IsNullOrWhiteSpace(data.AbilityId))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error,
                    $"AbilityRefHelper.ParseGameRef - missing or empty '{AbilityIdKey}'");
                return null;
            }

            EGameEvent? trigger = GameEventHelper.ToGameEvent(data.Trigger);
            if (trigger == null) return null;

            return (trigger.Value, data.AbilityId);
        }
    }
}
