using System;
using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler.List
{
    public class PluginListHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IConfig _config;
        private readonly IHypervisorFinder _finder;
        private readonly ILogger _log;

        public PluginListHandler(IConsole console, IConfig config, IUsage usage, IHypervisorFinder finder, ILogger log) : base(usage)
        {
            _console = console;
            _config = config;
            _finder = finder;
            _log = log;
        }

        public override string Group => "hypervisor";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling hypervisor list Command Handler with Args: {@args}", args);

            _console.Information("Available Hypervisor Plugins:");

            foreach (var hypervisor in _finder.Hypervisors)
            {
                _console.Information(string.Equals(_config.GetSetting("Hypervisor"), hypervisor,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? $" * {hypervisor}"
                    : $" - {hypervisor}");
            }
        }

        public override string UsageDescription => "Lists available hypervisor plugins.";
    }
}
