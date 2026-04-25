using SharedUtils.Source.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedUtils.Source.Logging
{
    public interface ILogger
    {
        void Log(LogPayload payload);
    }
}
