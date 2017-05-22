using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;

namespace VMLab.CommandHandler.VMControl
{
    public class DestroyHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;

        public DestroyHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IUsage usage) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"destroy", "d"};

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();

            foreach (var vm in _graphManager.VMs)
            {
                var control = _builder.GetVM(vm);

                if (control != null)
                {
                    _builder.DestroyVM(vm, control);
                }
            }
        }

        public override string UsageDescription => "Stops and deletes all VMs in current lab.";
    }
}
