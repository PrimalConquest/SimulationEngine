using SimulationEngine.Source.Data.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Units
{
    internal abstract class FieldUnit : Unit
    {
        Shape _occupation;
        protected FieldUnit()
        {
            
        }
    }
}
