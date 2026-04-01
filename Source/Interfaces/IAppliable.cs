using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Interfaces
{
    internal interface IAppliable
    {
        void Apply(Object source, Object target);
    }
}
