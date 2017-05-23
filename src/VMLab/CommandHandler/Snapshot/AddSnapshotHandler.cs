using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Snapshot
{
    public class AddSnapshotHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMBuilder _builder;

        public AddSnapshotHandler(IUsage usage, IConsole console, IScriptEngine scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IVMBuilder builder) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _builder = builder;
        }

        public override string Group => "snapshot";
        public override string[] Handles => new[] {"add", "a", "create"};
        public override void OnHandle(string[] args)
        {
            if (args.Length < 2)
            {
                _console.Error("Expected name parameter for snapshot. vmlab snapshot add <snapshotname>");
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

            foreach (var vm in vms.Select(v => _builder.GetVM(_graphManager.VMs.First(g => g.Name == v))))
            {
                vm.NewSnapshot(args[1]);
            }
        }

        public override string UsageDescription => "Adds a new snapshot to target VMs.";
    }
}
