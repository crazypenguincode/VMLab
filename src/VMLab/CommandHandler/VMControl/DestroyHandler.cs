using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.VMControl
{
    public class DestroyHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IConsole _console;
        private readonly IVMManager _vmManager;

        public DestroyHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IUsage usage, ISwitchParser switchParser, IConsole console, IVMManager vmManager) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _console = console;
            _vmManager = vmManager;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"destroy", "d"};

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
                vms = vms.Where(v => switches["vm"].Any(s => s == v.Name));

            if (!switches.ContainsKey("force") && !switches.ContainsKey("f"))
            {
                _console.Information("Are you sure you want to destroy virtual machines? (use -force to skip this in the future)");

                if (_console.ReadLine().ToLower() != "y")
                    return;
            }

            foreach (var vm in vms)
            {
                var control = _vmManager.GetVM(vm);

                if (control != null)
                {
                    _vmManager.DestroyVM(vm, control);
                }
            }
        }

        public override string UsageDescription => "Stops and deletes all VMs in current lab.";
    }
}
