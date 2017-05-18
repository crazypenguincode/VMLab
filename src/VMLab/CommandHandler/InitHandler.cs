using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;
using VMLab.Helper;

namespace VMLab.CommandHandler
{
    public class InitHandler : IParamHandler
    {
        public string Group => "root";

        private readonly IResource _resource;
        private readonly IFile _file;
        private readonly IConsole _console;

        public InitHandler(IResource resource, IFile file, IConsole console)
        {
            _resource = resource;
            _file = file;
            _console = console;
        }

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "init";
        }

        public void Handle(string[] args)
        {
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
	.TemplateFluentHandler(""__TEMPLATE__"")
	.Credential(""Admin"", ""Administrator"", ""P@ssw0rd01"")
	.Network(""NAT"")
	.CPU(1,2)
	.Memory(2048)
	.ShareFolder(""."", ""c:\\lab"");
";
            template = template.Replace("__TEMPLATE__", args[1]);

            _file.WriteAllText("vmlab.csx", template);
        }
    }
}
