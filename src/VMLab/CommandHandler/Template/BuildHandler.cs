using System;
using SystemInterface;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.CommandHandler
{
    public class BuildHandler : BaseParamHandler
    {
        public override string Group => "template";
        public override string[] Handles => new []{"build", "b"};

        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly IHypervisorCapabilityChecker _capabilityChecker;
        private readonly IEnvironment _environment;
        private readonly ITemplateManager _templateManager;

        public BuildHandler(IScriptEngine scriptEngine, IGraphManager graphManager, IConsole console, IHypervisorCapabilityChecker capabilityChecker, IEnvironment environment, IUsage usage, ITemplateManager templateManager) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _capabilityChecker = capabilityChecker;
            _environment = environment;
            _templateManager = templateManager;
        }

        public override void OnHandle(string[] args)
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

                if (_templateManager.CanBuild(template)) continue;

                _console.Error("Unable to build template! Check log for details...");
                return;
            }

            foreach (var t in _graphManager.Templates)
                _templateManager.Build(t, $"{_environment.CurrentDirectory}\\_vmlab\\template\\{Guid.NewGuid()}\\{t.Name}");
        }

        public override string UsageDescription => "Builds a template from vmlab.csx definition in current directory.";
    }
}
