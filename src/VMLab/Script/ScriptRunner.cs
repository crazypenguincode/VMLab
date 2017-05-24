using System;
using System.Linq;
using VMLab.Helper;

namespace VMLab.Script
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly IScriptEngine[] _engines;
        private readonly IConsole _console;
        public ScriptRunner(IScriptEngine[] engines, IConsole console)
        {
            _engines = engines;
            _console = console;
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

            
        }
    }
}
