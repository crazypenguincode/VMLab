using System.Diagnostics;
using System.Reflection;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class VersionHandler : BaseParamHandler
    {
        public override string Group => "root";
        public override string[] Handles => new[] {"version", "v"};
        private readonly IConsole _console;

        public VersionHandler(IConsole console, IUsage usage) : base(usage)
        {
            _console = console;
        }

        public override void OnHandle(string[] args)
        {

            // ReSharper disable once AssignNullToNotNullAttribute
            var versioninfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            _console.Information("{FileVersion}", versioninfo.FileVersion);
            _console.Information("{ProductVersion}", versioninfo.ProductVersion);
        }

        public override string UsageDescription => "Returns the version of VMLab.";
        public override string UsageName => "version";
    }
}
