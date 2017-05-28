using Serilog;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    /// <summary>
    /// Command handler that creates an initial vmlab.csx file.
    /// </summary>
    public class InitHandler : BaseParamHandler
    {
        public override string Group => "root";
        public override string[] Handles => new[] {"init", "i"};

        private readonly IConsole _console;
        private readonly ILogger _log;
        private readonly ILabManager _labManager;

        public InitHandler(IConsole console, IUsage usage, ILogger log, ILabManager labManager) : base(usage)
        {
            _console = console;
            _log = log;
            _labManager = labManager;
        }

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling init Command Handler with Args: {@args}", args);
            
            if (args.Length < 2)
            {
                _console.Error("You must pass a template name to initialise!");
                return;
            }

            _labManager.Init(args[1]);
        }

        public override string UsageDescription => "Create a vmlab.csx file with target template.";
    }
}
