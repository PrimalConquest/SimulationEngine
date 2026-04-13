using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Helpers.TargetingScheme;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories
{
    internal static class TargetingSchemeFactory
    {
        static Dictionary<string, ITargetingScheme> _parsedSchemes = new();

        public static ITargetingScheme? GetTargetingScheme(string schemeId)
        {
            if(schemeId == "null") return null;

            if(_parsedSchemes.ContainsKey(schemeId))
            {
                return _parsedSchemes[schemeId];
            }

            TargetingSchemeData? data = TargetingSchemeHelper.Parse(schemeId);
            if (data == null) return null;

            //move to factory
            if (string.IsNullOrWhiteSpace(data.Class))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"TargetingSchemeFactory.GetTargetingScheme - TargetingScheme '{schemeId}' has no 'class' field defined.");
                return null;
            }

            var type = Type.GetType("SimulationEngine.Source.Data.TargetingSchemes." + data.Class);
            ITargetingScheme? scheme = (ITargetingScheme)Activator.CreateInstance(type);

            if (scheme == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"TargetingSchemeFactory.GetTargetingScheme - Unknown ability class '{data.Class}' in '{schemeId}'");
                return null;
            }

            scheme.Extract(data.Specific);

            _parsedSchemes.Add(schemeId, scheme);

            return scheme;
        }
    }
}
