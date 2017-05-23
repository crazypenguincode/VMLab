using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.GUI
{
    public class GUIHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;

        public GUIHandler(IUsage usage, IConsole console, IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"gui", "ui", "g"};
        public override void OnHandle(string[] args)
        {
            if (args.Length < 2)
            {
                _console.Error("Expected vm parameter. vmlab.exe gui <vmname>");
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

            var control = _builder.GetVM(vm);

            if (control == null)
            {
                _console.Error("Can't get gui for vm because it hasn't been provisioned yet. Please run vmlab.exe start first.");
                return;
            }

            control.ShowUI();
        }

        public override string UsageDescription => "Shows the hypervisor user interface for target vm.";
    }
}
