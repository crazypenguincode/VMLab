using System.Linq;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Credentials
{
    /// <summary>
    /// Commandline handler which adds new secure credentials for a VM.
    /// </summary>
    public class AddCredentialHandler : BaseParamHandler
    {
        private readonly ICredentialManager _credentialManager;
        private readonly IScriptRunner _scriptRunner;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IConsole _console;

        public AddCredentialHandler(IUsage usage, ICredentialManager credentialManager, IScriptRunner scriptRunner, IGraphManager graphManager, ISwitchParser switchParser, IConsole console) : base(usage)
        {
            _credentialManager = credentialManager;
            _scriptRunner = scriptRunner;
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

            _scriptRunner.Execute();

            foreach (var vm in _graphManager.VMs.Where(v => switches["vm"].Select(n => n.ToLower()).Contains(v.Name.ToLower())))
            {
               _credentialManager.AddSecureCredential(cred, vm);
            }
        }

        public override string UsageDescription => "Adds a new credential to target virtual machines.";
    }
}
