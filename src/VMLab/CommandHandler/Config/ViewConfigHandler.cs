using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler.Config
{
    public class ViewConfigHandler : BaseParamHandler
    {
        private readonly IConfig _config;
        private readonly IConsole _console;
        private readonly ILogger _log;

        public ViewConfigHandler(IUsage usage, IConfig config, IConsole console, ILogger log) : base(usage)
        {
            _config = config;
            _console = console;
            _log = log;
        }

        public override string Group => "config";
        public override string[] Handles => new[] {"view", "v", "dump"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling Config list Command Handler with Args: {@args}", args);

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
