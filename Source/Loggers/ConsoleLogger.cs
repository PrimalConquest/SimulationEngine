using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Loggers
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(LogPayload payload)
        {
            Console.WriteLine(payload);
        }
    }
}
