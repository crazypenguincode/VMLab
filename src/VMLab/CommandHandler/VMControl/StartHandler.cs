using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;

namespace VMLab.CommandHandler
{
    /// <summary>
    /// Command Handler that starts or provisons virtual machines.
    /// </summary>
    public class StartHandler : BaseParamHandler
    {
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;
        private readonly ITemplateManager _templateManager;
        private readonly IManifestManager _manifestManager;
        private readonly ILogger _log;

        public StartHandler(IScriptRunner scriptEngine, IGraphManager graphManager, IConsole console, IUsage usage, ISwitchParser switchParser, IVMManager vmManager, ITemplateManager templateManager, IManifestManager manifestManager, ILogger log) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _switchParser = switchParser;
            _vmManager = vmManager;
            _templateManager = templateManager;
            _manifestManager = manifestManager;
            _log = log;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"start"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling start Command Handler with Args: {@args}", args);

            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
                vms = vms.Where(v => switches["vm"].Any(s => s == v.Name));

            foreach (var vm in vms)
            {
                var control = _vmManager.GetVM(vm);

                if (control == null)
                {
                    var templates = _manifestManager.GetInstalledTemplateManifests().Where(t => t.Name == vm.Template);

                    if (vm.Version != "latest")
                        templates = templates.Where(t => t.Version == vm.Version);

                    if (!templates.Any())
                    {
                        _console.Error("Can't create lab as Template {template} doesn't exist!", vm.Template);
                        return;
                    }

                    _templateManager.BuildVMFromTemplate(vm);
                }
                else
                {
                    if(control.PowerState != VMPower.Off)
                        continue;

                    control.Start();
                }
            }
        }

        public override string UsageDescription => "Creates lab environment and powers VMs on.";

    }
}
