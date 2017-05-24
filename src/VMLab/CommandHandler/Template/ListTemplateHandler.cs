using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class ListTemplateHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IManifestManager _manifestManager;

        public ListTemplateHandler(IConsole console, IUsage usage, IManifestManager manifestManager)  : base(usage)
        {
            _console = console;
            _manifestManager = manifestManager;
        }

        public override string Group => "template";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            _console.Information("Templates:");
            foreach (var template in _manifestManager.GetInstalledTemplateManifests())
            {
                _console.Information("{name} - {version}", template.Name, template.Version);
            }
        }

        public override string UsageDescription => "Gets list of available templates.";
    }
}
