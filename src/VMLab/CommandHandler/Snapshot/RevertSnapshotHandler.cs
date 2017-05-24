using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Snapshot
{
    public class RevertSnapshotHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;
        private readonly ILogger _log;

        public RevertSnapshotHandler(IUsage usage, IConsole console, IScriptRunner scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IVMManager vmManager, ILogger log) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _vmManager = vmManager;
            _log = log;
        }

        public override string Group => "snapshot";
        public override string[] Handles => new[] {"revert", "restore"};
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling snapshot revert Command Handler with Args: {@args}", args);

            if (args.Length < 2)
            {
                _console.Error("Expected name parameter for snapshot. vmlab snapshot revert <snapshotname>");
                return;
            }

            var switches = _switchParser.Parse(args.Skip(2).ToArray());

            _scriptEngine.Execute();

            var vms = _graphManager.VMs.Select(v => v.Name).ToArray();

            if (switches.ContainsKey("vm"))
            {
                vms = switches["vm"].ToArray();
            }

            if (vms.Any(v => _graphManager.VMs.All(g => v != g.Name)))
            {
                _console.Error("You have supplied a VM name that doesn't exist in vmlab.csx.");
                return;
            }

            foreach (var vm in vms.Select(v => _vmManager.GetVM(_graphManager.VMs.First(g => g.Name == v))))
            {
                if (vm.GetSnapshots().Contains(args[1]))
                    vm.RevertToSnapshot(args[1]);
            }
        }

        public override string UsageDescription => "Reverts lab VMs back to target snapshot.";
    }
}
