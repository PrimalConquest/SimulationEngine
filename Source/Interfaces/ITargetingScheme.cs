using SimulationEngine.Source.Data.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Interfaces
{
    internal interface ITargetingScheme
    {
        List<Unit> GatherTargets(Unit referenceUnit);
    }
}
