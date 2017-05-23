using System.Linq;
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
        private readonly ISwitchParser _switchParser;

        public StartHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console, IUsage usage, ISwitchParser switchParser) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
            _switchParser = switchParser;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"start"};

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
