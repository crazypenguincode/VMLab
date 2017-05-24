using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Threading;
using VMLab.Contract;
using VMLab.Contract.Helpers;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Hypervisor.VMwareWorkstation.VMX;
using IConsole = VMLab.Helper.IConsole;

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
        private readonly IConsole _console;

        public LabManager(IDirectory directory, IEnvironment environment, IVIX vix, IThread thread, Func<IVMXCollection> vmxFactory, ICompressHelper compressHelper, IConsole console)
        {
            _directory = directory;
            _environment = environment;
            _vix = vix;
            _thread = thread;
            _vmxFactory = vmxFactory;
            _compressHelper = compressHelper;
            _console = console;
        }

        public void ExportLab(string path)
        {
            _console.Information("Exporting lab to {path}", path);

            foreach (var vmx in _directory.GetFiles(_environment.CurrentDirectory, "*.vmx",
                SearchOption.AllDirectories))
            {
                _console.Information("Full cloning vm {vmx}", vmx);
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

            _console.Information("Compressing directory...");
            _compressHelper.CreateFromDirectory(_environment.CurrentDirectory, path, CompressionLevel.Optimal, false, Encoding.UTF8, filter => true);
            _console.Information("Compression completed.");
        }

        public void ImportLab(string path)
        {
            _console.Information("Importing lab from {path}", path);
            _compressHelper.ExtractToFolder(path, _environment.CurrentDirectory);
        }
    }
}
