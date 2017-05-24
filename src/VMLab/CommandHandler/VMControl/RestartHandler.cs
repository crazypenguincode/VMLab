using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;

namespace VMLab.CommandHandler.VMControl
{
    public class RestartHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;

        public RestartHandler(IUsage usage, ISwitchParser switchParser, IGraphManager graphManager, IScriptEngine scriptEngine, IVMManager vmManager) : base(usage)
        {
            _switchParser = switchParser;
            _graphManager = graphManager;
            _scriptEngine = scriptEngine;
            _vmManager = vmManager;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"restart", "reboot"};
        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
                vms = vms.Where(v => switches["vm"].Any(s => s == v.Name));

            foreach (var vm in vms)
            {
                var control = _vmManager.GetVM(vm);
                control?.Restart(switches.ContainsKey("force") || switches.ContainsKey("f"));
            }
        }

        public override string UsageDescription => "Restarts target virtual machines.";
    }
}
