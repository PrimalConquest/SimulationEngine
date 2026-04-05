using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Enums.EventTypes
{
    internal enum EUnitEvent
    {
        GetStat,
        TryActivate,
        Activate,
        Die,
        Spawn,
        Retreat,
        Move,
        RecieveDamage
    }
}
