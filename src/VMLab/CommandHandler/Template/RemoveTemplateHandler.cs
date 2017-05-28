using Serilog;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{

    /// <summary>
    /// Command Handler that removes target template.
    /// </summary>
    public class RemoveTemplateHandler : BaseParamHandler
    {
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _log;
        private readonly IConsole _console;

        public RemoveTemplateHandler(IUsage usage, ITemplateManager templateManager, ILogger log, IConsole console) : base(usage)
        {
            _templateManager = templateManager;
            _log = log;
            _console = console;
        }

        public override string Group => "template";
        public override string[] Handles => new []{"remove", "r"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template remove Command Handler with Args: {@args}", args);

            if (args.Length < 2)
            {
                _console.Error("Expected name of template to remove. vmlab.exe template remove <template name>");
                return;
            }

            _templateManager.RemoveTemplate(args[1]);
        }

        public override string UsageDescription => "Removes a template from vmlab.";
    }
}
