using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.TargetingSchemes
{
    internal class TargetCommander : ITargetingScheme
    {
        static string Targeting = "Targeting";

        ETargeting _targeting;

        public void Extract(JObject spec)
        {
            foreach (var prop in spec.Properties())
            {
                if (prop.Name == Targeting)
                {
                    string? str = prop.Value.Value<string>();
                    if(str == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"TargetCommander.Extract invalid value for fieald {Targeting}");
                        return;
                    }
                    ETargeting? temp = TargetingHelper.ToTargeting(str);
                    if (temp == null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"TargetCommander.Extract cannot parse fieald {Targeting}");
                        return;
                    }
                    _targeting = temp.Value;
                    break;
                }
            }
        }

        public List<Unit> GatherTargets(Unit referenceUnit)
        {
            Player currentPlayer = SimulationSystem.ActiveGame.CurrentPlayer;
            Player otherPlayer = SimulationSystem.ActiveGame.OtherPlayer;
            bool CurrentIsOwner = currentPlayer == referenceUnit.OwningPlayer;

            switch(_targeting)
            {
                case ETargeting.Ally: return new List<Unit> { (CurrentIsOwner ? currentPlayer : otherPlayer).Commander};
                case ETargeting.Enemy: return new List<Unit> { (CurrentIsOwner ? otherPlayer : currentPlayer).Commander };
                default: return new();
            }

           
        }
    }
}
