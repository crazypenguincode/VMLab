using System;
using SystemInterface;
using Serilog;
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

        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly IHypervisorCapabilityChecker _capabilityChecker;
        private readonly IEnvironment _environment;
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _log;


        public BuildHandler(IScriptRunner scriptEngine, IGraphManager graphManager, IConsole console, IHypervisorCapabilityChecker capabilityChecker, IEnvironment environment, IUsage usage, ITemplateManager templateManager, ILogger log) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _capabilityChecker = capabilityChecker;
            _environment = environment;
            _templateManager = templateManager;
            _log = log;
        }

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template build Command Handler with Args: {@args}", args);

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
