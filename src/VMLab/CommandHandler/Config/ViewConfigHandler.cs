using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Helper;

namespace VMLab.CommandHandler.Config
{
    public class ViewConfigHandler : BaseParamHandler
    {
        private readonly IConfig _config;
        private readonly IConsole _console;

        public ViewConfigHandler(IUsage usage, IConfig config, IConsole console) : base(usage)
        {
            _config = config;
            _console = console;
        }

        public override string Group => "config";
        public override string[] Handles => new[] {"view", "v", "dump"};

        public override void OnHandle(string[] args)
        {
            if (args.Length != 2)
            {
                _console.Error("Expected vmlab config view <scope>");
                return;
            }

            switch (args[1].ToLower())
            {
                case "system":
                    _console.Information(_config.Dump(ConfigScope.System));
                    break;
                case "user":
                    _console.Information(_config.Dump(ConfigScope.User));
                    break;
                case "lab":
                    _console.Information(_config.Dump(ConfigScope.Lab));
                    break;
                default:
                    _console.Error("Invalid scope. Expected system, user or lab!");
                    break;
            }
        }

        public override string UsageDescription => "Prints the contents of the config to the console.";
    }
}
