using VMLab.Contract;

namespace VMLab.CommandHandler.Import
{
    public class ImportTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;

        public ImportTemplateHandler(IUsage usage, ITemplateManager templateManager) : base(usage)
        {
            _templateManager = templateManager;
        }

        public override string Group => "template";
        public override string[] Handles => new[] { "import", "i", "add" };

        public override void OnHandle(string[] args)
        {
            _templateManager.ImportTemplate(args[1]);
        }

        public override string UsageDescription => "Imports a template into the VMLab template store!";
    }
}
