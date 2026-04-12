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
        TryPromote,
        Promote,
        Die,
        Draft,
        Retreat,
        TryMove,
        Move,
        TryFall,
        Fall,
        TryDisplace,
        Displace,
        RecieveDamage,
        RecieveEffect
    }
}
