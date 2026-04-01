using SimulationEngine.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Interfaces
{
    public interface ILogger
    {
        void Log(LogPayload payload);
    }
}
