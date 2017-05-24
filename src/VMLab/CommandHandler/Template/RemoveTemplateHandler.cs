using Serilog;
using VMLab.Contract;

namespace VMLab.CommandHandler.Template
{
    public class RemoveTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _log;

        public RemoveTemplateHandler(IUsage usage, ITemplateManager templateManager, ILogger log) : base(usage)
        {
            _templateManager = templateManager;
            _log = log;
        }

        public override string Group => "template";
        public override string[] Handles => new []{"remove", "r"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template remove Command Handler with Args: {@args}", args);

            _templateManager.RemoveTemplate(args[1]);
        }

        public override string UsageDescription => "Removes a template from vmlab.";
    }
}
