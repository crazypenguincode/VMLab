using Serilog;
using VMLab.Contract;

namespace VMLab.CommandHandler.Import
{
    public class ImportTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _log;

        public ImportTemplateHandler(IUsage usage, ITemplateManager templateManager, ILogger log) : base(usage)
        {
            _templateManager = templateManager;
            _log = log;
        }

        public override string Group => "template";
        public override string[] Handles => new[] { "import", "i", "add" };

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template import Command Handler with Args: {@args}", args);

            _templateManager.ImportTemplate(args[1]);
        }

        public override string UsageDescription => "Imports a template into the VMLab template store!";
    }
}
