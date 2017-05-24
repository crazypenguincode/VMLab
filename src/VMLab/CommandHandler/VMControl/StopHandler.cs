using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StopHandler : BaseParamHandler
    {
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;
        private readonly ILogger _log;

        public StopHandler(IScriptRunner scriptEngine, IGraphManager graphManager, IUsage usage, ISwitchParser switchParser, IVMManager vmManager, ILogger log) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _vmManager = vmManager;
            _log = log;
        }

        public override string Group => "root";
        public override string[] Handles => new[] { "stop"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling stop Command Handler with Args: {@args}", args);

            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
                vms = vms.Where(v => switches["vm"].Any(s => s == v.Name));

            foreach (var vm in vms)
            {
                var control = _vmManager.GetVM(vm);
                control?.Stop(switches.ContainsKey("force") || switches.ContainsKey("f"));
            }
        }

        public override string UsageDescription => "Shutdown all virtual machines in lab. ";
    }
}
