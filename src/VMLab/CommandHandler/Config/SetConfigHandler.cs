using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler.Config
{
    /// <summary>
    /// Command line handler which sets values in configuration files.
    /// </summary>
    public class SetConfigHandler : BaseParamHandler
    {
        private readonly IConfig _config;
        private readonly IConsole _console;
        private readonly ILogger _log;


        public SetConfigHandler(IUsage usage, IConfig config, IConsole console, ILogger log) : base(usage)
        {
            _config = config;
            _console = console;
            _log = log;
        }

        public override string Group => "config";
        public override string[] Handles => new[] {"set", "s"};
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling Config Set Command Handler with Args: {@args}", args);

            if (args.Length != 4)
            {
                _console.Error("Expected vmlab config set <scope> <valuename> <value>");
                return;
            }

            switch (args[1].ToLower())
            {
                case "system":
                    _config.WriteSetting(args[2], args[3], ConfigScope.System);
                    break;
                case "user":
                    _config.WriteSetting(args[2], args[3], ConfigScope.User);
                    break;
                case "lab":
                    _config.WriteSetting(args[2], args[3], ConfigScope.Lab);
                    break;
                default:
                    _console.Error("Invalid scope. Expected system, user or lab!");
                    break;
            }
        }

        public override string UsageDescription => "Sets a configuration setting.";
    }
}
