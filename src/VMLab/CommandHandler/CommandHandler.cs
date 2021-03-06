﻿using System.Collections.Generic;
using System.Linq;
using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    /// <summary>
    /// This class is the entry point for handling all commandline arguments.
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        private readonly IParamHandler[] _handlers;
        private readonly IConsole _console;
        private readonly IUsage _usage;
        private readonly ILogger _log;

        public CommandHandler(IEnumerable<IParamHandler> handlers, IConsole console, IUsage usage, ILogger log)
        {
            _console = console;
            _usage = usage;
            _log = log;
            _handlers = handlers.ToArray();
        }

        /// <summary>
        /// Parses commandline arguments and dispatches them to the relevant handler.
        /// </summary>
        /// <param name="args">Command arguments passed to the Main method.</param>
        public void Parse(string[] args)
        {
            _log.Information("Calling Command Handler with Args: {@args}", args);

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
