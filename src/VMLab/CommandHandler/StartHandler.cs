using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class StartHandler : IParamHandler
    {
        private readonly IScriptEngine _scriptEngine;

        public StartHandler(IScriptEngine scriptEngine)
        {
            _scriptEngine = scriptEngine;
        }

        public string Group => "root";

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "start";
        }

        public void Handle(string[] args)
        {
            _scriptEngine.Execute();

        }
    }
}
