using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories.Commands.CommandInfos
{
    public class PlaceSpecialCommandInfo : ICommandInfo
    {
        public string UnitId { get; set; } = "";
        public Cell Position { get; set; }
    }
}
