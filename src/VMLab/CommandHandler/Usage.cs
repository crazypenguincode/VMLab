using System.Linq;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class Usage : IUsage
    {
        private readonly IConsole _console;

        public Usage(IConsole console)
        {
            _console = console;
      }

        public void WriteUsage(IParamHandler[] handlers)
        {
            _console.Information("Usage:");
            _console.Information("vmlab (command) [subcommand] [switches]");
            _console.Information("");
            _console.Information("Available commands: (type -h after command for more help information)");

            foreach (var h in handlers.Where(h => h.Group == "root"))
            {
                _console.Information("\t{command}\t - {help}", h.UsageName, h.UsageDescription);
            }
        }

        public void WriteHubUsage(IParamHandler handler, IParamHandler[] handlers)
        {
            _console.Information("Usage:");
            _console.Information("vmlab {subcommand} [subcommand] [switches]", handler.UsageName);
            _console.Information("");
            _console.Information("Available sub commands: (type -h after command for more help information)");

            foreach (var h in handlers.Where(h => h.Group == handler.UsageName))
            {
                _console.Information("\t{command}\t - {help}", h.UsageName, h.UsageDescription);
            }
        }

        public void WriteCommandUsage(IParamHandler handler)
        {
            _console.Information("Usage:");
            if(handler.Group == "root")
                _console.Information("vmlab {subcommand} [switches]", handler.UsageName);
            else
                _console.Information("vmlab {command} {subcommand} [switches]", handler.Group, handler.UsageName);
            
            _console.Information("");
            _console.Information("Available switches:");

            foreach (var s in handler.UsageItems)
            {
                _console.Information("\t{command}\t - {help}", s.Key, s.Value);
            }
        }
    }
}
