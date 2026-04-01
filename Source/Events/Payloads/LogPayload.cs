using SimulationEngine.Source.Enums.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Events.Payloads
{
    public class LogPayload : EventPayload
    {
        public ELogCategory Category { get; set; }
        public ELogLevel Level { get; set; }
        public string Message { get; set; }

        public LogPayload(ELogCategory category, ELogLevel level, string message)
        {
            Category = category;
            Level = level;
            Message = message;
        }

        public override string ToString()
        {
            return $"[{Category.ToString()}] {Level.ToString()} : {Message}";
        }
    }
}
