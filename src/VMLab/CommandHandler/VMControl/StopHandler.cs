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

        public StopHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console, IUsage usage) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
        }

        public override string Group => "root";
        public override string[] Handles => new[] { "stop"};

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            foreach (var vm in _graphManager.VMs)
            {
                var control = _builder.GetVM(vm);
                control?.Stop();
            }
        }

        public override string UsageDescription => "Shutdown all virtual machines in lab. ";
    }
}
