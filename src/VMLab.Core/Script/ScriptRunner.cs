using System;
using System.Linq;
using Serilog;
using VMLab.GraphModels;
using VMLab.Helper;

namespace VMLab.Script
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly IScriptEngine[] _engines;
        private readonly IConsole _console;
        private readonly ILogger _log;
        private readonly IGraphManager _graphManager;

        public ScriptRunner(IScriptEngine[] engines, IConsole console, ILogger log, IGraphManager graphManager)
        {
            _engines = engines;
            _console = console;
            _log = log;
            _graphManager = graphManager;
        }

        public void Execute()
        {
            var engine = _engines.FirstOrDefault(e => e.CanHandle);

            if (engine == null)
            {
                _console.Error("Can't find a VMLab file in the current directory!");
                throw new ApplicationException("Can't find vmlab script file handled by current script engines!");
            }

            engine.Execute();

            _log.Information("Generated graph: {@graph}", _graphManager);
        }
    }
}
