using System.Linq;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Actions
{
    /// <summary>
    /// Allows user to run actions defined in the vmlab.csx file.
    /// </summary>
    public class ActionHandler : BaseParamHandler
    {
        private readonly IScriptRunner _scriptRunner;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly ISessionFactory _sessionFactory;
        private readonly ILogger _log;

        public ActionHandler(IUsage usage, IScriptRunner scriptRunner, IGraphManager graphManager, IConsole console, ISessionFactory sessionFactory, ILogger log) : base(usage)
        {
            _scriptRunner = scriptRunner;
            _graphManager = graphManager;
            _console = console;
            _sessionFactory = sessionFactory;
            _log = log;
        }

        public override string Group => "root";
        public override string[] Handles => new[] {"action", "a", "act", "do"};
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling Action Command Handler with Args: {@args}", args);
            if (args.Length < 2)
            {
                _console.Error("expected name of action to run!");
                return;
            }

            _scriptRunner.Execute();

            var actions = _graphManager.Actions.Where(a => a.Name == args[1]);

            _log.Information("Calling all action handlers for {action}", args[1]);

            foreach (var act in actions)
            {
                _log.Information("Running action handler.");
                act.OnAction(args.Skip(2).ToArray(), _sessionFactory.Build(_graphManager));
            }
        }

        public override string UsageDescription => "Executes an action from the vmlab.csx file.";

    }
}
