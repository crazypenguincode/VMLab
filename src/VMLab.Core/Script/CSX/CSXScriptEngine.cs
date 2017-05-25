using System;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Serilog;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Script
{
    public class CSXScriptEngine : IScriptEngine
    {
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly IScriptGlobal _global;
        private readonly IConsole _console;
        private readonly ILogger _log;

        public CSXScriptEngine(IFile file, IEnvironment environment, IScriptGlobal global, IConsole console, ILogger log)
        {
            _file = file;
            _environment = environment;
            _global = global;
            _console = console;
            _log = log;
        }

        public bool CanHandle => _file.Exists($"{_environment.CurrentDirectory}\\vmlab.csx");
        public void Execute()
        {
            _log.Information("Start exection of vmlab.csx");

            var scriptText = _file.ReadAllText("vmlab.csx");

            var script = CSharpScript.Create(scriptText, globalsType: typeof(IScriptGlobal))
                .WithOptions(ScriptOptions.Default
                    .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("VMLab")))
                    .WithImports("VMLab.Script"));

            try
            {
                var result = script.RunAsync(_global);
                result.Wait();
            }
            catch (Exception e)
            {
                _console.Error(e, "Error in script: {e}", e);
                throw new ApplicationException("Script failed to run!", e);
            }

        }
    }
}
