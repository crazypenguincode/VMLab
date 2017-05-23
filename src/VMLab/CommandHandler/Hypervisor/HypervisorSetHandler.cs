using System;
using System.Linq;
using VMLab.Helper;

namespace VMLab.CommandHandler.Hypervisor
{
    public class HypervisorSetHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IConfig _config;
        private readonly IHypervisorFinder _finder;

        public HypervisorSetHandler(IUsage usage, IConsole console, IConfig config, IHypervisorFinder finder) : base(usage)
        {
            _console = console;
            _config = config;
            _finder = finder;
        }

        public override string Group => "hypervisor";
        public override string[] Handles => new[] {"set", "s"};
        public override void OnHandle(string[] args)
        {
            if (args.Length < 2)
            {
                _console.Error("Expected hypervisor parameter. vmlab.exe hypervisor set <hypervisor name>.");
                _console.Error("To see a list of available hypervisors run vmlab.exe hypervisor list.");
                return;
            }

            var hypervisor = _finder.Hypervisors.FirstOrDefault(h => string.Equals(args[1], h, StringComparison.CurrentCultureIgnoreCase));

            if (hypervisor == null)
            {
                _console.Error("Invalid hypervisor name. Please use vmlab.exe hypervisor list to find valid names.");
                return;
            }

            _config.WriteSetting("Hypervisor", hypervisor, ConfigScope.System);
        }

        public override string UsageDescription => "Sets the hypervisor for vmlab to use.";
    }
}
