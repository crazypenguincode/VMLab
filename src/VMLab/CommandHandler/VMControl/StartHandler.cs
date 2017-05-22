using System.Collections.Generic;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StartHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;
        private readonly IConsole _console;

        public StartHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console, IUsage usage) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"start"};

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            foreach(var vm in _graphManager.VMs)
            {
                var control = _builder.GetVM(vm);

                if (control == null)
                {
                    if (!_builder.TemplateExist(vm.Template))
                    {
                        _console.Error("Can't create lab as Template {template} doesn't exist!", vm.Template);
                        return;
                    }

                    _builder.BuildVMFromTemplate(vm);
                }
                else
                {
                    control.Start();
                }
            }
        }

        public override string UsageDescription => "Creates lab environment and powers VMs on.";

    }
}
