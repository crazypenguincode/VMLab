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

        public PluginListHandler(IDirectory directory, IConsole console, IConfig config, IUsage usage) : base(usage)
        {
            _directory = directory;
            _console = console;
            _config = config;
        }

        public override string Group => "hypervisor";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            var asmFolder = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

            _console.Information("Available Hypervisor Plugins:");

            foreach (var file in _directory.GetFiles(asmFolder).Where(f => f !=null && Path.GetFileName(f).ToLower().StartsWith("vmlab.hypervisor.")))
            {
                if(file == null)
                    continue;

                if(!file.ToLower().EndsWith(".dll"))
                    continue;

                var hypervisor = Path.GetFileName(file).ToLower().Replace("vmlab.hypervisor.", "").Replace(".dll", "");
                hypervisor = hypervisor.Substring(0, 1).ToUpper() + hypervisor.Substring(1);

                _console.Information(string.Equals(_config.GetSetting("Hypervisor"), hypervisor,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? $" * {hypervisor}"
                    : $" - {hypervisor}");
            }
        }

        public override string UsageDescription => "Lists available hypervisor plugins.";
    }
}
