using Serilog;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Import
{
    /// <summary>
    /// Command Handler for importing templates into vmlab.
    /// </summary>
    public class ImportTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _log;
        private readonly IConsole _console;

        public ImportTemplateHandler(IUsage usage, ITemplateManager templateManager, ILogger log, IConsole console) : base(usage)
        {
            _templateManager = templateManager;
            _log = log;
            _console = console;
        }

        public override string Group => "template";
        public override string[] Handles => new[] { "import", "i", "add" };

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template import Command Handler with Args: {@args}", args);

            if (args.Length < 2)
            {
                _console.Error("Expected arguments. vmlab.exe template import <template archive path>");
                return;
            }

            _templateManager.ImportTemplate(args[1]);
        }

        public override string UsageDescription => "Imports a template into the VMLab template store!";
    }
}
