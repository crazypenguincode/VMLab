namespace VMLab.CommandHandler.Credentials
{
    public class CredentialHandler : HubParamHandler
    {
        public CredentialHandler(IUsage usage) : base(usage)
        {
        }

        public override string Group => "root";
        public override string UsageDescription => "Manages secure credentials for Virtual Machines.";
        public override string[] Handles => new[] {"credential", "credentials", "creds"};
        protected override string SubGroup => "credential";
    }
}
