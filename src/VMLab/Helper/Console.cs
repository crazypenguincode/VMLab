using System;
using Serilog;

namespace VMLab.Helper
{
    public class Console : IConsole
    {
        private readonly ILogger _log;

        public Console(ILogger log)
        {
            _log = log;
        }

        public void Information(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Information(e, message, props);
        }

        public void Information(string message, params object[] props)
        {
            _log.ForContext("Console", true).Information(message, props);
        }

        public void Warning(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(e, message, props);
        }

        public void Warning(string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(message, props);
        }

        public void Error(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Error(e, message, props);
        }

        public void Error(string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(message, props);
        }
    }
}
