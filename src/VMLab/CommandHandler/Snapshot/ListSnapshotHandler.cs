﻿using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Snapshot
{
    /// <summary>
    /// Command Handler that lists all snapshots in lab/target vms.
    /// </summary>
    public class ListSnapshotHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IVMManager _vmManager;
        private readonly ILogger _log;

        public ListSnapshotHandler(IUsage usage, IConsole console, IScriptRunner scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IVMManager vmManager, ILogger log) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _vmManager = vmManager;
            _log = log;
        }

        public override string Group => "snapshot";
        public override string[] Handles => new[] {"list", "l" };

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling snapshot list Command Handler with Args: {@args}", args);

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

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

            _console.Information("Current snapshots:");

            foreach (var vm in vms.Select(v => _graphManager.VMs.First(g => g.Name == v)))
            {
                var controller = _vmManager.GetVM(vm);

                _console.Information("[{vm}]", vm.Name);

                foreach (var snapshot in controller.GetSnapshots())
                {
                    _console.Information("- {snapshot}", snapshot);
                }

                _console.Information("");
            }
        }
        public override string UsageDescription => "List all snapshots in the lab.";
    }
}
