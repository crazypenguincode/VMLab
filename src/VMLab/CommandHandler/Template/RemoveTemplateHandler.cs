using VMLab.Contract;

namespace VMLab.CommandHandler.Template
{
    public class RemoveTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;

        public RemoveTemplateHandler(IUsage usage, ITemplateManager templateManager) : base(usage)
        {
            _templateManager = templateManager;
        }

        public override string Group => "template";
        public override string[] Handles => new []{"remove", "r"};

        public override void OnHandle(string[] args)
        {
            _templateManager.RemoveTemplate(args[1]);
        }

        public override string UsageDescription => "Removes a template from vmlab.";
    }
}
