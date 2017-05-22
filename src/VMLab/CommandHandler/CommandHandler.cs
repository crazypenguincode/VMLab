using System;
using System.Collections.Generic;
using System.Linq;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IParamHandler[] _handlers;
        private readonly IConsole _console;
        private readonly IUsage _usage;

        public CommandHandler(IEnumerable<IParamHandler> handlers, IConsole console, IUsage usage)
        {
            _console = console;
            _usage = usage;
            _handlers = handlers.ToArray();
        }

        public void Parse(string[] args)
        {
            var useableHandler = _handlers.FirstOrDefault(h => h.Group == "root" && h.CanHandle(args, _handlers));

            if (useableHandler != null)
                useableHandler.Handle(args);
            else
            {



                if (args.Length > 0)
                    if(args[0].ToLower() == "-help" || args[0].ToLower() == "-h" || args[0].ToLower() == "help")
                        _usage.WriteUsage(_handlers);
                    else
                        _console.Error("Unknown command or action {command}! Please run vmlab.exe -help for usage.", args[0]);
                else
                    _console.Error("Expected extra parameters. Please run vmlab.exe -help for usage.");
                
                    
            }
        }
    }
}
