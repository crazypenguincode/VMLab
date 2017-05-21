using System.Collections.Generic;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StartHandler : IParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;
        private readonly IConsole _console;

        public StartHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console)
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

            return args[0].ToLower() == "start";
        }

        public void Handle(string[] args)
        {
            _scriptEngine.Execute();

            foreach(var vm in _graphManager.VMs)
            {
                var control = _builder.GetVM(vm.Name);

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
    }
}
