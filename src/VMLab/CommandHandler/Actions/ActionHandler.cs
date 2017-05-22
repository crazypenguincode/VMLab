using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Actions
{
    public class ActionHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly ISessionFactory _sessionFactory;

        public ActionHandler(IUsage usage, IScriptEngine scriptEngine, IGraphManager graphManager, IConsole console, ISessionFactory sessionFactory) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _sessionFactory = sessionFactory;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"action", "a", "act", "do"};
        public override void OnHandle(string[] args)
        {
            if (args.Length < 2)
            {
                _console.Error("expected name of action to run!");
                return;
            }

            _scriptEngine.Execute();

            var actions = _graphManager.Actions.Where(a => a.Name == args[1]);

            foreach (var act in actions)
            {
                act.OnAction(args.Skip(2).ToArray(), _sessionFactory.Build(_graphManager));
            }
        }

        public override string UsageDescription => "Executes an action from the vmlab.csx file.";

    }
}
