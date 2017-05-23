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
        private readonly ISwitchParser _switchParser;

        public GUIHandler(IUsage usage, IConsole console, IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, ISwitchParser switchParser) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _switchParser = switchParser;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"gui", "ui", "g"};
        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
            {
                vms = _graphManager.VMs.Where(v => switches["vm"].Contains(v.Name));
            }

            foreach (var control in vms.Select(v => _builder.GetVM(v)).Where(v => v != null))
            {
                control.ShowUI();
            }
        }

        public override string UsageDescription => "Shows the hypervisor user interface for target vm.";
    }
}
