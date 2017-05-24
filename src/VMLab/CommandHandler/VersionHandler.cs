using System.Diagnostics;
using System.Reflection;
using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class VersionHandler : BaseParamHandler
    {
        public override string Group => "root";
        public override string[] Handles => new[] {"version", "v"};
        private readonly IConsole _console;
        private readonly ILogger _log;


        public VersionHandler(IConsole console, IUsage usage, ILogger log) : base(usage)
        {
            _console = console;
            _log = log;
        }

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling version Command Handler with Args: {@args}", args);

            // ReSharper disable once AssignNullToNotNullAttribute
            var versioninfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            _console.Information("{FileVersion}", versioninfo.FileVersion);
            _console.Information("{ProductVersion}", versioninfo.ProductVersion);
        }

        public override string UsageDescription => "Returns the version of VMLab.";
        public override string UsageName => "version";
    }
}
