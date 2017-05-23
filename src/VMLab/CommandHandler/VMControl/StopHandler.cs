﻿using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StopHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;
        private readonly IConsole _console;
        private readonly ISwitchParser _switchParser;

        public StopHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console, IUsage usage, ISwitchParser switchParser) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
            _switchParser = switchParser;
        }

        public override string Group => "root";
        public override string[] Handles => new[] { "stop"};

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            var vms = _graphManager.VMs;

            if (switches.ContainsKey("vm"))
                vms = vms.Where(v => switches["vm"].Any(s => s == v.Name));

            foreach (var vm in vms)
            {
                var control = _builder.GetVM(vm);
                control?.Stop(switches.ContainsKey("force") || switches.ContainsKey("f"));
            }
        }

        public override string UsageDescription => "Shutdown all virtual machines in lab. ";
    }
}
