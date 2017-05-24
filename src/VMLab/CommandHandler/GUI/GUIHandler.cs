using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;

namespace VMLab.CommandHandler.GUI
{
    public class GUIHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;

        public GUIHandler(IUsage usage, IScriptEngine scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IVMManager vmManager) : base(usage)
        {
       
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _vmManager = vmManager;
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

            foreach (var control in vms.Select(v => _vmManager.GetVM(v)).Where(v => v != null))
            {
                control.ShowUI();
            }
        }

        public override string UsageDescription => "Shows the hypervisor user interface for target vm.";
    }
}
