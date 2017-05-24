using System;
using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Exec
{
    public class PowershellHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMManager _vmManager;

        public PowershellHandler(IUsage usage, IConsole console, IScriptEngine scriptEngine, IGraphManager graphManager, IVMManager vmManager) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _vmManager = vmManager;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"powershell", "p", "posh", "ps" };
        public override void OnHandle(string[] args)
        {
            if (args.Length < 3)
            {
                _console.Error("Expected the following arguments vmlab.exe powershell <vmname> <script>");
                return;
            }

            _scriptEngine.Execute();

            var vm = _graphManager.VMs.FirstOrDefault(
                v => string.Equals(v.Name, args[1], StringComparison.CurrentCultureIgnoreCase));

            if (vm == default(VM))
            {
                _console.Error("Can't find a provisioned vm named {name}", args[1]);
                return;
            }

            var control = _vmManager.GetVM(vm);

            if (control == null)
            {
                _console.Error("Can't execute powershell script on vm because it hasn't been provisioned yet. Please run vmlab.exe start first.");
                return;
            }

            control.Powershell(args[2]);
        }

        public override string UsageDescription => "Executes powershell script in target vm.";
    }
}
