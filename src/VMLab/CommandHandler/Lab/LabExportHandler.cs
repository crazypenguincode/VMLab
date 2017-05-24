using System.Linq;
using SystemInterface.IO;
using Serilog;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;

namespace VMLab.CommandHandler.Lab
{
    public class LabExportHandler : BaseParamHandler
    {
        private readonly IScriptRunner _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly IFile _file;
        private readonly IVMManager _vmManager;
        private readonly ILabManager _labManager;
        private readonly ILogger _log;

        public LabExportHandler(IUsage usage, IScriptRunner scriptEngine, IGraphManager graphManager, IConsole console, IFile file, IVMManager vmManager, ILabManager labManager, ILogger log) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _file = file;
            _vmManager = vmManager;
            _labManager = labManager;
            _log = log;
        }

        public override string Group => "lab";
        public override string[] Handles => new[] {"export", "e"};
        public override void OnHandle(string[] args)
        {
            _log.Information("Calling lab export Command Handler with Args: {@args}", args);

            if (args.Length < 2)
            {
                _console.Error("Expected path of export archive. vmlab.exe lab export <pathtoarchive>");
                return;
            }

            if (!_file.Exists("vmlab.csx"))
            {
                _console.Error("You can only run this from a directory which has a vmlab.csx file in it!");
                return;
            }

            _scriptEngine.Execute();

            if (_graphManager.VMs
                    .Select(v => _vmManager.GetVM(v))
                    .Where(v => v != null).Any(v => v.PowerState != VMPower.Off))
            {
                _console.Error("You need to stop all of the virtual machines before you can export the lab!");
                return;
            }

            _labManager.ExportLab(args[1]);


        }

        public override string UsageDescription => "Exports lab to archive file.";
    }
}
