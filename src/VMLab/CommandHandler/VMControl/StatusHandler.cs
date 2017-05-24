using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.VMControl
{
    public class StatusHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMManager _vmManager;
        private readonly IConsole _console;
        private readonly IManifestManager _manifestManager;
        private readonly ICapabilities _capabilities;

        public StatusHandler(IUsage usage, IScriptEngine scriptEngine, IGraphManager graphManager, IVMManager vmManager, IConsole console, IManifestManager manifestManager, ICapabilities capabilities) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _vmManager = vmManager;
            _console = console;
            _manifestManager = manifestManager;
            _capabilities = capabilities;
        }

        public override string Group => "root";

        public override string[] Handles => new[] {"status", "stat"};
        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            _console.Information("Lab Status:");
            _console.Information("Hypervisor: {hypervisor}", _capabilities.Hypervisor);
            _console.Information("Name: {}", _graphManager.LabName);
            _console.Information("Description: {}", _graphManager.LabDescription);
            _console.Information("Author: {}", _graphManager.LabAuthor);
            _console.Information("");
            _console.Information("Locks:");
            foreach(var l in _graphManager.Locks)
                _console.Information(" - {lock}", l);

            _console.Information("");
            _console.Information("VMs:");

            foreach (var vm in _graphManager.VMs)
            {
                var control = _vmManager.GetVM(vm);
                var manifest = _manifestManager.GetManifestFromVM(vm);

                if (control == null)
                {
                    _console.Information("[{name}]", vm.Name);
                    _console.Information("Unprovisioned");
                    _console.Information("Template: {template} - {version}", vm.Template, vm.Version);
                    _console.Information("");
                    continue;
                }
                
                _console.Information("Power State: {state}", control?.PowerState);
                _console.Information("Memory: {memory}", control?.Memory);
                _console.Information("CPUs: {cpu}", control?.Cpu);
                _console.Information("Cpu Cores: {cores}", control?.CpuCore);
                _console.Information("OS: {os} - {arch}", control?.OS, control?.Arch);
                _console.Information("Template: {template} - {version}", manifest?.Name, manifest?.Version);
                _console.Information("Nested Virtualization: {nv}", vm.NestedVirtualization);
                _console.Information("");
            }
        }

        public override string UsageDescription => "Shows the status of virtual machines in the Lab environment.";
    }
}
