using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.CommandHandler.Lab
{
    public class LabImportHandler : BaseParamHandler
    {
        private readonly IConsole _console;
        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly ISwitchParser _switchParser;
        private readonly ILabManager _labManager;

        public LabImportHandler(IUsage usage, IConsole console, IDirectory directory, IEnvironment environment, ISwitchParser switchParser, ILabManager labManager) : base(usage)
        {
            _console = console;
            _directory = directory;
            _environment = environment;
            _switchParser = switchParser;
            _labManager = labManager;
        }

        public override string Group => "lab";

        public override string[] Handles => new []{"import", "i"};

        public override void OnHandle(string[] args)
        {
            if (args.Length < 2)
            {
                _console.Error("Expected archive parameter. vmlab.exe lab import <path to archive>");
                return;
            }

            var switches = _switchParser.Parse(args.Skip(2).ToArray());

            if (_directory.GetFiles(_environment.CurrentDirectory).Length > 0 ||
                _directory.GetDirectories(_environment.CurrentDirectory).Where(d => !d.EndsWith("_vmlab")).ToArray().Length > 0)
            {
                if (!switches.ContainsKey("force"))
                {
                    _console.Error("You can only run an import on a empty directory!");
                    return;
                }
            }

            _labManager.ImportLab(args[1]);

        }

        public override string UsageDescription => "Imports a lab into the current directory.";
    }
}
