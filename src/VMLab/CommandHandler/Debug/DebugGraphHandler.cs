using System.Collections.Generic;
using Newtonsoft.Json;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Debug
{
    public class DebugGraphHandler : IParamHandler
    {
        private readonly IGraphManager _graphManager;
        private readonly IScriptEngine _scriptEngine;
        private readonly IConsole _console;

        public DebugGraphHandler(IGraphManager graphManager, IScriptEngine scriptEngine, IConsole console)
        {
            _graphManager = graphManager;
            _scriptEngine = scriptEngine;
            _console = console;
        }

        public string Group => "debug";
        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "graph";
        }

        public void Handle(string[] args)
        {
            _scriptEngine.Execute();
            _console.Information(JsonConvert.SerializeObject(_graphManager, Formatting.Indented));
            _console.Pause("Press any key to continue...");
        }
    }
}
