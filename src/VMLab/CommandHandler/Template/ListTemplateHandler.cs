using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class ListTemplateHandler : BaseParamHandler
    {
        private readonly IConfig _config;
        private readonly IDirectory _directory;
        private readonly IConsole _console;

        public ListTemplateHandler(IConfig config, IDirectory directory, IConsole console, IUsage usage)  : base(usage)
        {
            _config = config;
            _directory = directory;
            _console = console;
        }

        public override string Group => "template";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            _console.Information("Templates:");
            foreach (var template in _directory.GetDirectories(_config.GetSetting("TemplateDir")))
            {
                _console.Information(Path.GetFileName(template));
            }
        }

        public override string UsageDescription => "Gets list of available templates.";
    }
}
