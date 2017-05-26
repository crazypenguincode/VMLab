namespace VMLab.CommandHandler.Config
{
    public class ConfigHandler : HubParamHandler
    {
        public ConfigHandler(IUsage usage) : base(usage)
        {
        }

        public override string Group => "root";
        public override string UsageDescription => "Handles vmlab configuration.";
        public override string[] Handles => new[] {"config", "c", "cfg", "configuration"};
        public override string SubGroup => "config";
    }
}
