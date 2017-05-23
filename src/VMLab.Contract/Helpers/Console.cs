using System;
using Serilog;

namespace VMLab.Helper
{
    public class Console : IConsole
    {
        private readonly ILogger _log;
        private readonly IConsole _console;

        public Console(ILogger log, IConsole console)
        {
            _log = log;
            _console = console;
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

        public void Pause(string message = "")
        {
            if(!string.IsNullOrEmpty(message))
                Information(message);

            System.Console.ReadKey();
        }

        public string ReadLine()
        {
            var input = _console.ReadLine();
            _log.Information("User entered: {input}", input);

            return input;
        }
    }
}
