using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using VMLab.Helper;

namespace VMLab.Script
{
    public class ScriptEngine : IScriptEngine
    {
        private readonly IFile _file;
        private readonly IScriptGlobal _global;
        private readonly IConsole _console;

        public ScriptEngine(IFile file, IScriptGlobal global, IConsole console)
        {
            _file = file;
            _global = global;
            _console = console;
        }

        public void Execute()
        {
            if(!_file.Exists("vmlab.csx"))
                throw new FileNotFoundException("vmlab.csx file doesn't exist in current directory!");

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
            }
        }
    }
}
