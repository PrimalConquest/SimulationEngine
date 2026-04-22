using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories.Commands.CommandInfos
{
    public class MoveCommandInfo : ICommandInfo
    {
        public Cell Position { get; set; }
        public EDirection Direction { get; set; }
    }
}
