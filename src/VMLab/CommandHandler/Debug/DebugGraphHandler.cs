using System.Collections.Generic;
using Newtonsoft.Json;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Debug
{
    public class DebugGraphHandler : BaseParamHandler
    {
        private readonly IGraphManager _graphManager;
        private readonly IScriptEngine _scriptEngine;
        private readonly IConsole _console;


        public DebugGraphHandler(IGraphManager graphManager, IScriptEngine scriptEngine, IConsole console, IUsage usage) : base(usage)
        {
            _graphManager = graphManager;
            _scriptEngine = scriptEngine;
            _console = console;
        }

        public override string Group => "debug";

        public override void OnHandle(string[] args)
        {
            _scriptEngine.Execute();
            _console.Information(JsonConvert.SerializeObject(_graphManager, Formatting.Indented));
            _console.Pause("Press any key to continue...");
        }

        public override string UsageDescription => "Prints the VM graph to the console.";
        public override string[] Handles => new[] { "graph", "g" };
    }
}
