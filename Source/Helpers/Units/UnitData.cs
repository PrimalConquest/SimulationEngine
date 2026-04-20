using SimulationEngine.Source.Helpers.Abilities;
using System.Collections.Generic;

namespace SimulationEngine.Source.Helpers.Units
{
    internal class UnitData
    {
        public string Color       { get; set; } = "";
        public string ShapeId     { get; set; } = "";
        public string StatSheetId { get; set; } = "";

        /// <summary>List of unit-event ability bindings.</summary>
        public List<AbilityRefData> AbilityList       { get; set; } = new();

        /// <summary>List of game-event (global) ability bindings.</summary>
        public List<AbilityRefData> GlobalAbilityList { get; set; } = new();
    }
}
