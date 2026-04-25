using SharedUtils.Source.Events.Payloads;
using SharedUtils.Source.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedUtils.Source.Logging.Loggers
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(LogPayload payload)
        {
            Console.WriteLine(payload);
        }
    }
}
