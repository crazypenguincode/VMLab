using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class ListTemplateHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IVMBuilder _builder;

        public ListTemplateHandler(IConsole console, IUsage usage, IVMBuilder builder)  : base(usage)
        {
            _console = console;
            _builder = builder;
        }

        public override string Group => "template";
        public override string[] Handles => new[] {"list", "l"};

        public override void OnHandle(string[] args)
        {
            _console.Information("Templates:");
            foreach (var template in _builder.GetInstalledTemplateManifests())
            {
                _console.Information("{name} - {version}", template.Name, template.Version);
            }
        }

        public override string UsageDescription => "Gets list of available templates.";
    }
}
