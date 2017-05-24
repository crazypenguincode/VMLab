﻿using System;
using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Exec
{
    public class ExecHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMManager _vmManager;

        public ExecHandler(IUsage usage, IConsole console, IScriptEngine scriptEngine, IGraphManager graphManager, IVMManager vmManager) : base(usage)
        {
            _console = console;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _vmManager = vmManager;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"exec", "e"};
        public override void OnHandle(string[] args)
        {
            if (args.Length < 3)
            {
                _console.Error("Expected the following arguments vmlab.exe exec <vmname> <command>");
                return;
            }

            var command = string.Join(" ", args.Skip(2));

            _scriptEngine.Execute();

            var vm = _graphManager.VMs.FirstOrDefault(
                v => string.Equals(v.Name, args[1], StringComparison.CurrentCultureIgnoreCase));

            if (vm == default(VM))
            {
                _console.Error("Can't find a provisioned vm named {name}", args[1]);
                return;
            }

            var control = _vmManager.GetVM(vm);

            if (control == null)
            {
                _console.Error("Can't execute command on vm because it hasn't been provisioned yet. Please run vmlab.exe start first.");
                return;
            }

            control.Exec("c:\\windows\\system32\\cmd.exe", $"/c {command}");
        }

        public override string UsageDescription => "Executes a command inside target vm.";
    }
}
