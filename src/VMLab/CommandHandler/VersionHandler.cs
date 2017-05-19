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

            // ReSharper disable once AssignNullToNotNullAttribute
            var versioninfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            _console.Information("{FileVersion}", versioninfo.FileVersion);
            _console.Information("{ProductVersion}", versioninfo.ProductVersion);
        }
    }
}
