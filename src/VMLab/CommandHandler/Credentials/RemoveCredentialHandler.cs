using System.Linq;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Script;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.CommandHandler.Credentials
{
    /// <summary>
    /// Commandline handler removes target stored credentails.
    /// </summary>
    public class RemoveCredentialHandler : BaseParamHandler
    {
        private readonly ICredentialManager _credentialManager;
        private readonly IScriptRunner _scriptRunner;
        private readonly IGraphManager _graphManager;
        private readonly ISwitchParser _switchParser;
        private readonly IConsole _console;

        public RemoveCredentialHandler(IUsage usage, ICredentialManager credentialManager, IScriptRunner scriptRunner, IGraphManager graphManager, ISwitchParser switchParser, IConsole console) : base(usage)
        {
            _credentialManager = credentialManager;
            _scriptRunner = scriptRunner;
            _graphManager = graphManager;
            _switchParser = switchParser;
            _console = console;
        }

        public override string Group => "credential";
        public override string[] Handles => new[] { "remove", "delete", "r"};
        public override void OnHandle(string[] args)
        {
            var switches = _switchParser.Parse(args.Skip(1).ToArray());

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

            _scriptRunner.Execute();

            foreach (var vm in _graphManager.VMs.Where(v => switches["vm"].Contains(v.Name)))
            {
                _credentialManager.RemoveSecureCredential(switches["group"].First(), vm);
            }
        }

        public override string UsageDescription => "Removes a secure password from target VM.";
    }
}
