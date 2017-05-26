namespace VMLab.CommandHandler.Lab
{
    public class LabHandler : HubParamHandler
    {
        public LabHandler(IUsage usage) : base(usage)
        {
        }

        public override string Group => "root";
        public override string UsageDescription => "Allows for importing and exporting of whole lab folder.";
        public override string[] Handles => new[] {"lab", "l"};
        public override string SubGroup => "lab";
    }
}
