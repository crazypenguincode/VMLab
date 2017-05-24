using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler.Config
{
    public class GetConfigHandler : BaseParamHandler
    {
        private readonly IConfig _config;
        private readonly IConsole _console;
        private readonly ILogger _log;

        public GetConfigHandler(IUsage usage, IConfig config, IConsole console, ILogger log) : base(usage)
        {
            _config = config;
            _console = console;
            _log = log;
        }

        public override string Group => "config";
        public override string[] Handles => new[] { "get", "g" };
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling Config Get Command Handler with Args: {@args}", args);

            if (args.Length != 3)
            {
                _console.Error("Expected vmlab config get <scope> <valuename>");
                return;
            }

            switch (args[1].ToLower())
            {
                case "system":
                    _console.Information(_config.GetSetting(args[2], ConfigScope.System));
                    break;
                case "user":
                    _console.Information(_config.GetSetting(args[2], ConfigScope.User));
                    break;
                case "lab":
                    _console.Information(_config.GetSetting(args[2], ConfigScope.Lab));
                    break;
                case "merged":
                    _console.Information(_config.GetSetting(args[2]));
                    break;
                default:
                    _console.Error("Invalid scope. Expected system, user, lab or merged!");
                    break;
            }
        }

        public override string UsageDescription => "Retrives configuration setting.";
    }
}
