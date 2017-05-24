using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Threading;
using VMLab.Contract;
using VMLab.Contract.Helpers;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Hypervisor.VMwareWorkstation.VMX;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class LabManager : ILabManager
    {
        private readonly IDirectory _directory;
        private readonly IEnvironment _environment;
        private readonly IVIX _vix;
        private readonly IThread _thread;
        private readonly Func<IVMXCollection> _vmxFactory;
        private readonly ICompressHelper _compressHelper;

        public LabManager(IDirectory directory, IEnvironment environment, IVIX vix, IThread thread, Func<IVMXCollection> vmxFactory, ICompressHelper compressHelper)
        {
            _directory = directory;
            _environment = environment;
            _vix = vix;
            _thread = thread;
            _vmxFactory = vmxFactory;
            _compressHelper = compressHelper;
        }

        public void ExportLab(string path)
        {
            foreach (var vmx in _directory.GetFiles(_environment.CurrentDirectory, "*.vmx",
                SearchOption.AllDirectories))
            {
                var folder = Path.GetDirectoryName(vmx);
                var vmxFile = Path.GetFileName(vmx) ?? "";
                _directory.CreateDirectory($"{folder}_full");

                var vm = _vix.ConnectToVM(vmx);
                _vix.Clone($"{folder}_full\\{vmxFile}", vm, null, false);
                _vix.CloseObject(vm);

                _thread.Sleep(1000);

                _directory.Delete(folder, true);
                _directory.Move($"{folder}_full", folder);

                //Stop vm name from having clone of in the name.
                var vmxData = _vmxFactory();
                vmxData.ReadFromFile(vmx);
                vmxData.WriteValue("displayName", vmxFile.Replace(".vmx", ""));
                vmxData.WriteToFile(vmx);
            }

            _compressHelper.CreateFromDirectory(_environment.CurrentDirectory, path, CompressionLevel.Optimal, false, Encoding.UTF8, filter => true);
        }

        public void ImportLab(string path)
        {
            _compressHelper.ExtractToFolder(path, _environment.CurrentDirectory);
        }
    }
}
