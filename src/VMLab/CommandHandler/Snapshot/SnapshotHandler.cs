namespace VMLab.CommandHandler.Snapshot
{
    public class SnapshotHandler : HubParamHandler
    {
        public SnapshotHandler(IUsage usage) : base(usage)
        {
        }

        public override string Group => "root";
        public override string UsageDescription => "Manages lab vm snapshots.";
        public override string[] Handles => new[] {"snapshot", "s"};
        protected override string SubGroup => "snapshot";
    }
}
