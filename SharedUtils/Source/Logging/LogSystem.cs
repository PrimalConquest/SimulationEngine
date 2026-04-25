using SharedUtils.Source.Events;
using SharedUtils.Source.Events.Busses;
using SharedUtils.Source.Events.Payloads;
using SharedUtils.Source.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedUtils.Source.Logging
{
    public static class LogSystem
    {
        private static void RegisterLoggers()
        {
            RegisterLogger(new ConsoleLogger());
        }

        private static IEventBus<ELogCategory, LogPayload> _onLog = new PriorityEventBus<ELogCategory, LogPayload>();

        public static void Log(ELogCategory category, ELogLevel level, string message)
        {
            //da dropva debug logove ako ne e v debug build (daje moje i warningite)

            LogPayload payload = new(category, level, message);
            _onLog.Raise(category, payload);
        }

        public static void RegisterLogger(ILogger Logger)
        {
            foreach (ELogCategory category in Enum.GetValues(typeof(ELogCategory)))
            {
                EventCallback<LogPayload> callback = new(Logger.Log);
                _onLog.AddListener(category, callback);
            }
        }

        private static void Init()
        {
            foreach (ELogCategory category in Enum.GetValues(typeof(ELogCategory)))
            {
                _onLog.RegisterChannel(category);
            }
        }

        static LogSystem()
        {
            Init();
            RegisterLoggers();
        }
    }
}
