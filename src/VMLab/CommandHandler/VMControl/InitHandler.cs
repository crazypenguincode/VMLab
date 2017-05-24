using SystemInterface.IO;
using Serilog;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class InitHandler : BaseParamHandler
    {
        public override string Group => "root";
        public override string[] Handles => new[] {"init", "i"};

        private readonly IResource _resource;
        private readonly IFile _file;
        private readonly IConsole _console;
        private readonly ILogger _log;

        public InitHandler(IResource resource, IFile file, IConsole console, IUsage usage, ILogger log) : base(usage)
        {
            _resource = resource;
            _file = file;
            _console = console;
            _log = log;
        }

        public override void OnHandle(string[] args)
        {
            _log.Information("Calling init Command Handler with Args: {@args}", args);

            if (_file.Exists("vmlab.csx"))
            {
                _console.Warning("Can't init vmlab.csx because it already exists!");
                return;
            }

            if (args.Length < 2)
            {
                _console.Error("You must pass a template to initialise!");
            }

            var template = @"VM(""myVM"")
	.Template(""__TEMPLATE__"")
	.Credential(""Admin"", ""Administrator"", ""P@ssw0rd01"")
	.Network(""NAT"")
	.CPU(1,2)
	.Memory(2048)
	.ShareFolder(""."", ""c:\\lab"");
";
            template = template.Replace("__TEMPLATE__", args[1]);

            _file.WriteAllText("vmlab.csx", template);
        }

        public override string UsageDescription => "Create a vmlab.csx file with target template.";
    }
}
