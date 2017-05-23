using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SystemInterface.IO;
using VMLab.Helper;

namespace VMLab.CommandHandler.List
{
    public class PluginListHandler : BaseParamHandler
    {
        private readonly IDirectory _directory;
        private readonly IConsole _console;
        private readonly IConfig _config;
        private readonly IHypervisorFinder _finder;

        public PluginListHandler(IDirectory directory, IConsole console, IConfig config, IUsage usage, IHypervisorFinder finder) : base(usage)
        {
            _directory = directory;
            _console = console;
            _config = config;
            _finder = finder;
        }

        public override string Group => "hypervisor";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
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
