using Newtonsoft.Json.Linq;
using SharedUtils.Source.Logging;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Helpers
{
    internal static class ValueRefHelper
    {
        const string TypeKey   = "Type";
        const string ValueKey  = "Value";
        const string SourceKey = "Source";

        public static IValueRef? Parse(JToken token)
        {
            if (token.Type != JTokenType.Object)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ValueRefHelper.Parse - expected an object with a '{TypeKey}' field, got '{token.Type}'");
                return null;
            }

            JObject obj = (JObject)token;

            string? typeStr = obj[TypeKey]?.Value<string>();
            if (string.IsNullOrWhiteSpace(typeStr))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ValueRefHelper.Parse - missing or empty '{TypeKey}' field");
                return null;
            }

            EValueRefType? type = ValueRefTypeHelper.ToValueRefType(typeStr);
            if (type == null) return null;

            switch (type.Value)
            {
                case EValueRefType.Flat:
                {
                    JToken? valueToken = obj[ValueKey];
                    if (valueToken == null || valueToken.Type != JTokenType.Integer)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ValueRefHelper.Parse - '{EValueRefType.Flat}' requires an integer '{ValueKey}' field");
                        return null;
                    }
                    return new FlatValueRef(valueToken.Value<int>());
                }

                case EValueRefType.Relative:
                {
                    string? sourceStr = obj[SourceKey]?.Value<string>();
                    if (string.IsNullOrWhiteSpace(sourceStr))
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ValueRefHelper.Parse - '{EValueRefType.Relative}' requires a string '{SourceKey}' field");
                        return null;
                    }

                    EValueSource? source = ValueSourceHelper.ToValueSource(sourceStr);
                    if (source == null) return null;

                    return new RelativeValueRef(source.Value);
                }

                default:
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ValueRefHelper.Parse - unhandled EValueRefType '{type.Value}'");
                    return null;
            }
        }
    }
}
