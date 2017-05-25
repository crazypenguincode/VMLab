using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler.Credentials
{
    public class ListCredentialHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IScriptRunner _scriptRunner;
        private readonly IGraphManager _graphManager;
        private readonly ICredentialManager _credentialManager;

        public ListCredentialHandler(IUsage usage, IGraphManager graphManager, IConsole console, ICredentialManager credentialManager, IScriptRunner scriptRunner) : base(usage)
        {
            _graphManager = graphManager;
            _console = console;
            _credentialManager = credentialManager;
            _scriptRunner = scriptRunner;
        }

        public override string Group => "credential";
        public override string[] Handles => new[] {"list"};
        public override void OnHandle(string[] args)
        {
            _scriptRunner.Execute();

            _console.Information("Credentials:");
            foreach (var vm in _graphManager.VMs)
            {
                _credentialManager.LoadSecureCredentials(vm);

                _console.Information("[{vm}]", vm.Name);
                foreach(var c in _credentialManager.AllCredentials(vm))
                    _console.Information("\t Group: {group,-20} Username: {username,-30} Secure: {secure}", c.Group, c.Username, c.Secure);
            }
            _console.Information("");
        }

        public override string UsageDescription => "Lists all credentials being used by ";
    }
}
