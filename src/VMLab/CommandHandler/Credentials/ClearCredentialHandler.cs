using System.Linq;
using VMLab.Contract.CredentialManager;
using VMLab.Helper;

namespace VMLab.CommandHandler.Credentials
{
    /// <summary>
    /// Command handler for clearing all secure credentials. Useful for when exporting lab.
    /// </summary>
    public class ClearCredentialHandler : BaseParamHandler
    {
        private readonly ISwitchParser _switchParser;
        private readonly ICredentialManager _credentialManager;
        private readonly IConsole _console;

        public ClearCredentialHandler(IUsage usage, ICredentialManager credentialManager, ISwitchParser switchParser, IConsole console) : base(usage)
        {
            _credentialManager = credentialManager;
            _switchParser = switchParser;
            _console = console;
        }

        public override string Group => "credential";
        public override string[] Handles => new[] { "clear", "nuke" };
        public override void OnHandle(string[] args)
        {
            var switches = _switchParser.Parse(args.Skip(1).ToArray());

            if (!switches.ContainsKey("force") && !switches.ContainsKey("f"))
            {
                _console.Information("Are you sure you want to delete all the credentials in this lab?");
                if (_console.ReadLine().ToLower() != "y")
                    return;
            }

            _credentialManager.ClearAllSecureCredentail();
        }

        public override string UsageDescription => "Clears all secure credentials from a lab.";
    }
}
