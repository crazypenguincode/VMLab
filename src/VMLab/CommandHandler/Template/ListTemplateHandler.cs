using Serilog;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    /// <summary>
    /// Command Handler lists details for installed templates.
    /// </summary>
    public class ListTemplateHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IManifestManager _manifestManager;
        private readonly ILogger _log;

        public ListTemplateHandler(IConsole console, IUsage usage, IManifestManager manifestManager, ILogger log)  : base(usage)
        {
            _console = console;
            _manifestManager = manifestManager;
            _log = log;
        }

        public override string Group => "template";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling template list Command Handler with Args: {@args}", args);
            
            _console.Information("Templates:");
            foreach (var template in _manifestManager.GetInstalledTemplateManifests())
            {
                _console.Information("{name} - {version}", template.Name, template.Version);
            }
        }

        public override string UsageDescription => "Gets list of available templates.";
    }
}
