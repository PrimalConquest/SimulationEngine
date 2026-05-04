using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories.Commands.CommandInfos
{
    public class ActivateSpecialCommandInfo : ICommandInfo
    {
        public string UnitId { get; set; } = "";
    }
}
