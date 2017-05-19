using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.CommandHandler
{
    public class BuildHandler : IParamHandler
    {
        public string Group => "root";

        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IVMBuilder _builder;
        private readonly IConsole _console;
        private readonly IHypervisorCapabilityChecker _capabilityChecker;
        private readonly IEnvironment _environment;

        public BuildHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IVMBuilder builder, IConsole console, IHypervisorCapabilityChecker capabilityChecker, IEnvironment environment)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _builder = builder;
            _console = console;
            _capabilityChecker = capabilityChecker;
            _environment = environment;
        }

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "build";
        }

        public void Handle(string[] args)
        {
            _scriptEngine.Execute();

            foreach (var template in _graphManager.Templates)
            {
                var result = _capabilityChecker.CheckTemplate(template);

                if (!result.Item1)
                {
                    _console.Error(result.Item2);
                    return;
                }

                if (_builder.CanBuild(template)) continue;

                _console.Error("Unable to build template! Check log for details...");
                return;
            }

            foreach (var t in _graphManager.Templates)
                _builder.Build(t, $"{_environment.CurrentDirectory}\\_vmlab\\template\\{t.Name}");
        }
    }
}
