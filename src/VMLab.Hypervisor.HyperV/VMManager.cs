using System;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Hypervisor.HyperV.HyperV;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV
{
    public class VMManager : IVMManager
    {
        private readonly IHyperV _hyperV;
        private readonly IVMLoader _vmLoader;
        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly IFile _file;

        public VMManager(IHyperV hyperV, IVMLoader vmLoader, IDirectory directory, IEnvironment environment, IFile file)
        {
            _hyperV = hyperV;
            _vmLoader = vmLoader;
            _directory = directory;
            _environment = environment;
            _file = file;
        }

        public IVMControl GetVM(VM vm)
        {
            if (!_directory.Exists($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\"))
                return null;

            return (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\manifest.json")
                select _vmLoader.LoadByName(vm.Name, $"{dir}\\{vm.Name}", model: vm)).FirstOrDefault();


        }

        public void DestroyVM(VM vm, IVMControl control)
        {
            _hyperV.DestroyVM(vm.Name);
        }

        public void PreStart(VM vm)
        {
            throw new NotImplementedException();
        }

        public void PostStart(IVMControl control, VM vm)
        {
            throw new NotImplementedException();
        }
    }
}
