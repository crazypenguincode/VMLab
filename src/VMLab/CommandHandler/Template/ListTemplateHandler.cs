using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class ListTemplateHandler : IParamHandler
    {
        private readonly IConfig _config;
        private readonly IDirectory _directory;
        private readonly IConsole _console;

        public ListTemplateHandler(IConfig config, IDirectory directory, IConsole console)
        {
            _config = config;
            _directory = directory;
            _console = console;
        }

        public string Group => "template";
        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "list";
        }

        public void Handle(string[] args)
        {
            _console.Information("Templates:");
            foreach (var template in _directory.GetDirectories(_config.GetSetting("TemplateDir")))
            {
                _console.Information(Path.GetFileName(template));
            }
        }
    }
}
