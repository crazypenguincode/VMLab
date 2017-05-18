using System;
using Newtonsoft.Json;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class DebugHandler : IParamHandler
    {
        private readonly IConsole _console;
        private readonly IGraphManager _graphManager;
        private readonly IScriptEngine _scriptEngine;

        public DebugHandler(IConsole console, IGraphManager graphManager, IScriptEngine scriptEngine)
        {
            _console = console;
            _graphManager = graphManager;
            _scriptEngine = scriptEngine;
        }

        public bool RootCommand => true;
        public bool CanHandle(string[] args)
        {
            if (args.Length < 2)
                return false;

            return args[0].ToLower() == "debug";
        }

        public void Handle(string[] args)
        {
            switch (args[1].ToLower())
            {
                case "graph":
                    DebugGraph();
                    break;
                default:
                    _console.Information($"{args[1]} is an unknown debug command!");
                    break;
            }
            _console.Pause("Press any key to continue...");
        }

        private void DebugGraph()
        {
            _scriptEngine.Execute();
            _console.Information(JsonConvert.SerializeObject(_graphManager, Formatting.Indented));
        }
    }
}
