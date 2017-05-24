using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class VMManager : IVMManager
    {
        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly IVMLoader _loader;

        public VMManager(IDirectory directory, IEnvironment environment, IFile file, IVMLoader loader)
        {
            _directory = directory;
            _environment = environment;
            _file = file;
            _loader = loader;
        }

        public IVMControl GetVM(GraphModels.VM vm)
        {
            if (!_directory.Exists($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\"))
                return null;

            return (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\{vm.Name}.vmx")
                select _loader.GetVMFromPath($"{dir}\\{vm.Name}\\{vm.Name}.vmx", vm.Credentials)).FirstOrDefault();
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

            _directory.Delete(vmfolder, true);
        }
    }
}
