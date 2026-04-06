using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Enums.EventTypes
{
    public enum EUnitEvent
    {
        GetStat,
        TryActivate,
        Activate,
        Die,
        Draft,
        Retreat,
        Move,
        RecieveDamage,
        RecieveEffect
    }
}
