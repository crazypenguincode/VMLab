using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StopHandler : IParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;
        private readonly IConsole _console;

        public StopHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
        }

        public string Group => "root";

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "stop";
        }

        public void Handle(string[] args)
        {
            _scriptEngine.Execute();

            foreach (var vm in _graphManager.VMs)
            {
                var control = _builder.GetVM(vm.Name);
                control?.Stop();
            }
        }
    }
}
