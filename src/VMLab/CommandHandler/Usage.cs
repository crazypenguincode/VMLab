using System.Linq;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    /// <summary>
    /// Renders command usage information.
    /// </summary>
    public class Usage : IUsage
    {
        private readonly IConsole _console;

        public Usage(IConsole console)
        {
            _console = console;
        }

        /// <summary>
        /// Writes overall usage information to the console.
        /// Only handlers in the root group are rendered.
        /// </summary>
        /// <param name="handlers">Pass in all handlers that are in the system.</param>
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

        /// <summary>
        /// Used to render hub commands and thier sub commands.
        /// </summary>
        /// <param name="hub">Hub parameter handler to render usage for.</param>
        /// <param name="handlers">Pass in all handlers that are in the system</param>
        public void WriteHubUsage(IParamHandler hub, IParamHandler[] handlers)
        {
            _console.Information("Usage:");
            _console.Information("vmlab {subcommand} [subcommand] [switches]", hub.UsageName);
            _console.Information("");
            _console.Information("Available sub commands: (type -h after command for more help information)");

            foreach (var h in handlers.Where(h => h.Group == hub.UsageName))
            {
                _console.Information("\t{command}\t - {help}", h.UsageName, h.UsageDescription);
            }
        }

        /// <summary>
        /// Writes usage for a specific command.
        /// </summary>
        /// <param name="handler">handler to write usage for.</param>
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
