using Newtonsoft.Json.Linq;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;
using System.Linq;

namespace SimulationEngine.Source.Data.TargetingSchemes
{
    internal class TargetSpecialUnits : ITargetingScheme
    {
        static readonly string TargetingKey = "Targeting";

        ETargeting _targeting = ETargeting.Ally;

        public void Extract(JObject spec)
        {
            foreach (var prop in spec.Properties())
            {
                if (prop.Name != TargetingKey) continue;

                string? str = prop.Value.Value<string>();
                if (str == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"TargetSpecialUnits.Extract - invalid value for field '{TargetingKey}'");
                    return;
                }

                ETargeting? parsed = TargetingHelper.ToTargeting(str);
                if (parsed == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"TargetSpecialUnits.Extract - cannot parse field '{TargetingKey}' value '{str}'");
                    return;
                }

                _targeting = parsed.Value;
                break;
            }
        }

        public List<Unit> GatherTargets(Unit referenceUnit)
        {
            Player currentPlayer = SimulationSystem.ActiveGame.CurrentPlayer;
            Player otherPlayer  = SimulationSystem.ActiveGame.OtherPlayer;
            bool currentIsOwner = currentPlayer == referenceUnit.OwningPlayer;

            Player targetPlayer = _targeting == ETargeting.Ally
                ? (currentIsOwner ? currentPlayer : otherPlayer)
                : (currentIsOwner ? otherPlayer  : currentPlayer);

            EColor color = referenceUnit.Color;

            return targetPlayer.SpecialUnits.Values
                .Where(u => u.Color == color)
                .ToList();
        }
    }
}
