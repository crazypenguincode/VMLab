using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class VersionHandler : IParamHandler
    {
        public string Group => "root";

        private readonly IConsole _console;

        public VersionHandler(IConsole console)
        {
            _console = console;
        }

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "version";
        }

        public void Handle(string[] args)
        {
            var asm = Assembly.GetExecutingAssembly();

            if (asm.Location == null)
                return;

            var filever = FileVersionInfo.GetVersionInfo(asm.Location);

            _console.Information("Version: {Version}", filever.FileVersion);
        }
    }
}
