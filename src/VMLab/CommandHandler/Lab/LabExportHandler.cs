using System.Linq;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;

namespace VMLab.CommandHandler.Lab
{
    public class LabExportHandler : BaseParamHandler
    {
        private readonly IScriptEngine _scriptEngine;
        private readonly IGraphManager _graphManager;
        private readonly IConsole _console;
        private readonly IVMBuilder _builder;
        private readonly IFile _file;

        public LabExportHandler(IUsage usage, IScriptEngine scriptEngine, IGraphManager graphManager, IConsole console, IVMBuilder builder, IFile file) : base(usage)
        {
            _scriptEngine = scriptEngine;
            _graphManager = graphManager;
            _console = console;
            _builder = builder;
            _file = file;
        }

        public override string Group => "lab";
        public override string[] Handles => new[] {"export", "e"};
        public override void OnHandle(string[] args)
        {
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
                    .Select(v => _builder.GetVM(v))
                    .Where(v => v != null).Any(v => v.PowerState != VMPower.Off))
            {
                _console.Error("You need to stop all of the virtual machines before you can export the lab!");
                return;
            }

            _builder.ExportLab(args[1]);


        }

        public override string UsageDescription => "Exports lab to archive file.";
    }
}
