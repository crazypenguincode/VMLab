using VMLab.Contract;

namespace VMLab.CommandHandler.Lab
{
    /// <summary>
    /// Command Handler that cleans up a lab folder (removes _vmlab folder).
    /// </summary>
    public class CleanHandler : BaseParamHandler
    {

        private readonly ILabManager _labManager;

        public CleanHandler(ILabManager labManager, IUsage usage) : base(usage)
        {
            _labManager = labManager;
        }

        public override string Group => "lab";
        public override string[] Handles => new[] {"clean"};
        public override void OnHandle(string[] args)
        {
            _labManager.Clean();
        }

        public override string UsageDescription => "Cleans lab and removes all files created by vmlab!";
    }
}
