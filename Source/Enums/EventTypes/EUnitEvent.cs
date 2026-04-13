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
        TryDraft,
        Draft,
        TryRetreat,
        Retreat,
        TryMove,
        Move,
        TryFall,
        Fall,
        TryDisplace,
        Displace,
        TryOverride,
        Override,
        RecieveDamage,
        RecieveEffect
    }
}
