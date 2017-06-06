using System;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.HyperV
{
  
    public class LabManager : ILabManager
    {

        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly IConsole _console;


        public LabManager(IDirectory directory, IEnvironment environment, IFile file, IConsole console)
        {
            _directory = directory;
            _environment = environment;
            _file = file;
            _console = console;
        }

        public void ExportLab(string path)
        {
            throw new NotImplementedException();
        }

        public void ImportLab(string path)
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }

        public void Init(string templateName)
        {
            if (_file.Exists("vmlab.csx"))
            {
                _console.Warning("Can't init vmlab.csx because it already exists!");
                return;
            }

            var template = @"VM(""myVM"")
	.Template(""__TEMPLATE__"")
	.Credential(""Admin"", ""Administrator"", ""P@ssw0rd01"")
	.Network(""NAT"")
	.CPU(1,2)
	.Memory(2048)
	.ShareFolder(""."", ""c:\\lab"");
";
            template = template.Replace("__TEMPLATE__", templateName);

            _file.WriteAllText("vmlab.csx", template);
        }
    }
}
