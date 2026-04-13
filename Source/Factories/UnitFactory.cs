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
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories
{
    public static class UnitFactory
    {
        static Dictionary<string, Unit> _parsedUnits = new();

        public static Unit? GetUnit(string unitId, Player owner)
        {
            if (_parsedUnits.ContainsKey(unitId))
            {
                return _parsedUnits[unitId].DeepCopy(owner);
            }

            UnitData? data = UnitHelper.Parse(unitId);
            if (data == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit cannot parse unit with id '{unitId}'");
                return null;
            }

            EColor? color = ColorHelper.ToColor(data.Color);
            if(color == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse color '{data.Color}'");
                return null;
            }
            Shape shape = ShapeFactory.GetShape(data.ShapeId);
            StatSheet? sheet = StatSheetFactory.GetSheet(data.StatSheetId);
            if (sheet == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse stat sheet '{data.StatSheetId}'");
                return null;
            }

            Unit unit = new(owner, color.Value, sheet, shape);

            foreach (KeyValuePair<string, string> pair in data.AbilityMap)
            {
                EUnitEvent? unitEvent = UnitEventHelper.ToUnitEvent(pair.Key);
                if (unitEvent == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse unit event '{pair.Key}' for ability '{pair.Value}'");
                    continue;
                }
                Ability? ability = AbilityFactory.GetAbility(pair.Value, unit);
                if (ability == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse ability '{pair.Value}'");
                    continue;
                }
                unit.GrantAbility(unitEvent.Value, ability);
            }

            foreach (KeyValuePair<string, string> pair in data.GlobalAbilityMap)
            {
                EGameEvent? gameEvent = GameEventHelper.ToGameEvent(pair.Key);
                if (gameEvent == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse game event '{pair.Key}' for ability '{pair.Value}'");
                    continue;
                }
                Ability? ability = AbilityFactory.GetAbility(pair.Value, unit);
                if (ability == null)
                {
                    LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"UnitFactory.GetUnit - Cannot parse ability '{pair.Value}'");
                    continue;
                }
                unit.GrantGlobalAbility(gameEvent.Value, ability);
            }

            _parsedUnits.Add(unitId, unit);
            return unit;
        }
    }
}
