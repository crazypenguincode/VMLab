using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;

namespace VMLab.CommandHandler.GUI
{
    /// <summary>
    /// Command handler that shows the hypervisor gui for target vm.
    /// </summary>
    public class GUIHandler : BaseParamHandler
    {
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;
        private readonly ILogger _log;

        public GUIHandler(IUsage usage, IScriptRunner scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IVMManager vmManager, ILogger log) : base(usage)
        {
       
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _vmManager = vmManager;
            _log = log;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"gui", "ui", "g"};
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling gui Command Handler with Args: {@args}", args);

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
