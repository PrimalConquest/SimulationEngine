using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Helpers.Units
{
    internal class UnitData
    {
        public string Color { get; set; } = "";
        public string ShapeId { get; set; } = "";
        public string StatSheetId { get; set; } = "";
        public Dictionary<string, string> AbilityMap { get; set; } = new ();
        public Dictionary<string, string> GlobalAbilityMap { get; set; } = new();
    }
}
