using SimulationEngine.Source.Data.Abilities;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Helpers;
using SimulationEngine.Source.Helpers.Abilities;
using SimulationEngine.Source.Helpers.Enums;
using SimulationEngine.Source.Helpers.Units;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;
using System.Collections.Generic;

namespace SimulationEngine.Source.Factories
{
    public static class UnitFactory
    {
        static Dictionary<string, Unit> _parsedUnits = new();

        public static Unit? GetUnit(string unitId, Player owner)
        {
            if (_parsedUnits.ContainsKey(unitId))
                return _parsedUnits[unitId].DeepCopy(owner);

            UnitData? data = UnitHelper.Parse(unitId);
            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"UnitFactory.GetUnit cannot parse unit with id '{unitId}'");
                return null;
            }

            EColor? color = ColorHelper.ToColor(data.Color);
            if (color == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"UnitFactory.GetUnit - Cannot parse color '{data.Color}'");
                return null;
            }

            Shape shape = ShapeFactory.GetShape(data.ShapeId);

            StatSheet? sheet = StatSheetFactory.GetSheet(data.StatSheetId);
            if (sheet == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                    $"UnitFactory.GetUnit - Cannot parse stat sheet '{data.StatSheetId}'");
                return null;
            }

            Unit unit = new(owner, color.Value, sheet, shape);

            // Unit-event abilities
            foreach (AbilityRefData refData in data.AbilityList)
            {
                var parsed = AbilityRefHelper.ParseUnitRef(refData);
                if (parsed == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                        $"UnitFactory.GetUnit - Cannot parse unit ability ref (Trigger='{refData.Trigger}', AbilityId='{refData.AbilityId}')");
                    continue;
                }

                Ability? ability = AbilityFactory.GetAbility(parsed.Value.abilityId, unit);
                if (ability == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                        $"UnitFactory.GetUnit - Cannot load ability '{parsed.Value.abilityId}'");
                    continue;
                }

                unit.GrantAbility(parsed.Value.trigger, ability);
            }

            // Global (game-event) abilities
            foreach (AbilityRefData refData in data.GlobalAbilityList)
            {
                var parsed = AbilityRefHelper.ParseGameRef(refData);
                if (parsed == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                        $"UnitFactory.GetUnit - Cannot parse global ability ref (Trigger='{refData.Trigger}', AbilityId='{refData.AbilityId}')");
                    continue;
                }

                Ability? ability = AbilityFactory.GetAbility(parsed.Value.abilityId, unit);
                if (ability == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning,
                        $"UnitFactory.GetUnit - Cannot load global ability '{parsed.Value.abilityId}'");
                    continue;
                }

                unit.GrantGlobalAbility(parsed.Value.trigger, ability);
            }

            _parsedUnits.Add(unitId, unit);
            return unit;
        }
    }
}
