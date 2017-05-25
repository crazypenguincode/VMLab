using System.Linq;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Credentials
{
    public class AddCredentialHandler : BaseParamHandler
    {
        private readonly ICredentialManager _credentialManager;
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IConsole _console;

        public AddCredentialHandler(IUsage usage, ICredentialManager credentialManager, IScriptEngine scriptEngine, IGraphManager graphManager, ISwitchParser switchParser, IConsole console) : base(usage)
        {
            _credentialManager = credentialManager;
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _console = console;
        }

        public override string Group => "credential";
        public override string[] Handles => new[] {"add", "a", "new"};
        public override void OnHandle(string[] args)
        {
            var switches = _switchParser.Parse(args.Skip(1).ToArray());
            string password;

            if (!switches.ContainsKey("vm"))
            {
                _console.Error("Missing -vm switch");
                return;
            }

            if (!switches.ContainsKey("group"))
            {
                _console.Error("Missing -group switch!");
                return;
            }

            if (!switches.ContainsKey("username"))
            {
                _console.Error("Missing -username switch");
                return;
            }

            if (!switches.ContainsKey("password"))
            {
                _console.Information("Please enter password:");
                password = _console.ReadPassword();
            }
            else
            {
                password = switches["password"].First();
            }

            var cred = new Credential
            {
                Group = switches["group"].First(),
                Username = switches["username"].First(),
                Password = password
            };

            _scriptEngine.Execute();

            foreach (var vm in _graphManager.VMs.Where(v => switches["vm"].Contains(v.Name)))
            {
                _credentialManager.AddSecureCredential(cred, vm);
            }
        }

        public override string UsageDescription => "Adds a new credential to target virtual machines.";
    }
}
