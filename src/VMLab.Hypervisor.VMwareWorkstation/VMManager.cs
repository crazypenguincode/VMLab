using System;
using System.IO;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.Hypervisor.VMwareWorkstation.VMX;
using VMLab.Script.FluentInterface;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class VMManager : IVMManager
    {
        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly IVMLoader _loader;
        private readonly IOnStartProvisioner _onStartProvisioner;
        private readonly Func<IVMXCollection> _vmxFactory;
        private readonly IConsole _console;

        public VMManager(IDirectory directory, IEnvironment environment, IFile file, IVMLoader loader, IOnStartProvisioner onStartProvisioner, Func<IVMXCollection> vmxFactory, IConsole console)
        {
            _directory = directory;
            _environment = environment;
            _file = file;
            _loader = loader;
            _onStartProvisioner = onStartProvisioner;
            _vmxFactory = vmxFactory;
            _console = console;
        }

        public IVMControl GetVM(GraphModels.VM vm)
        {
            if (!_directory.Exists($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\"))
                return null;

            return (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\{vm.Name}.vmx")
                select _loader.GetVMFromPath($"{dir}\\{vm.Name}\\{vm.Name}.vmx", null, vm)).FirstOrDefault();
        }

        public void DestroyVM(GraphModels.VM vm, IVMControl control)
        {
            var vmfolder = (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\{vm.Name}.vmx")
                select dir).First();

            if (control.PowerState != VMPower.Off)
            {
                control.Stop(true);
            }

            try
            {
                _directory.Delete(vmfolder, true);
            }
            catch (IOException)
            {
                _console.Information("Can't clean up VM {vm} folder. You will need to remove it manually.", vm.Name);
            }
            
        }

        public void PreStart(GraphModels.VM vm)
        {
            var vmxpath = (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\{vm.Name}.vmx")
                       select $"{dir}\\{vm.Name}\\{vm.Name}.vmx").First();
            var vmx = _vmxFactory();

            vmx.ReadFromFile(vmxpath);
            _onStartProvisioner.PreStart(vmx, vm);
            vmx.WriteToFile(vmxpath);
        }

        public void PostStart(IVMControl control, GraphModels.VM vm)
        {
            _onStartProvisioner.PostStart(control, vm);
        }
    }
}
