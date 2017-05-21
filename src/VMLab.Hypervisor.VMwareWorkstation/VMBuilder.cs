using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Threading;
using VixCOM;
using VMLab.Contract;
using VMLab.Contract.Helpers;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Hypervisor.VMwareWorkstation.DiskHelpers;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Hypervisor.VMwareWorkstation.VM;
using VMLab.Hypervisor.VMwareWorkstation.VMX;
using VMLab.Script.FluentInterface;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class VMBuilder : IVMBuilder
    {
        private readonly Func<IVMXCollection> _vmxFactory;
        private readonly IHardDriveBuilder _driveBuilder;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IPVNHelper _ipvnHelper;
        private readonly IGuestOSTranslator _osTranslator;
        private readonly IEnvironment _environment;
        private readonly IFileDownloader _fileDownloader;
        private readonly IConsole _console;
        private readonly IVMLoader _loader;
        private readonly ICompressHelper _compressHelper;
        private readonly IThread _thread;
        private readonly IConfig _config;
        private readonly IVIX _vix;

        public VMBuilder(Func<IVMXCollection> vmxFactory, IHardDriveBuilder driveBuilder, IDirectory directory,  IPVNHelper ipvnHelper, IGuestOSTranslator osTranslator, IEnvironment environment, IFile file, IFileDownloader fileDownloader, IConsole console, IVMLoader loader, ICompressHelper compressHelper, IThread thread, IConfig config, IVIX vix)
        {
            _vmxFactory = vmxFactory;
            _driveBuilder = driveBuilder;
            _directory = directory;
            _ipvnHelper = ipvnHelper;
            _osTranslator = osTranslator;
            _environment = environment;
            _file = file;
            _fileDownloader = fileDownloader;
            _console = console;
            _loader = loader;
            _compressHelper = compressHelper;
            _thread = thread;
            _config = config;
            _vix = vix;
        }

        public bool CanBuild(Template template)
        {
            if (template.CPUCores < 1)
                return false;
            

            if (template.CPUs < 1)
                return false;

            if (template.HardDisks.Count > 16)
                return false;

            return template.Memory >= 1;
        }

        public void Build(Template template, string templateFolder)
        {
            var vmxpath = $"{templateFolder}\\{template.Name}.vmx";

            GenerateVMFiles(template, templateFolder, vmxpath);

            var vm = _loader.GetVMFromPath(vmxpath) as VMControl;

            if(vm == null)
                throw new NullReferenceException("Can't load VM controller object!");

            vm.SetCredentials(template.Credentials);
            vm.SetCredentials("Admin");

            vm.Start();

            if (!template.HeadLess)
                vm.ShowUI();

            _console.Information("Waiting for c:\\vmlab.ready file to be created in vm!");
            vm.WaitFile("c:\\vmlab.ready");
            vm.DeleteFileFromVM("c:\\vmlab.ready");
            vm.Restart();

            _console.Information("Running provisioning script!");
            template.OnProvision?.Invoke(vm);

            _console.Information("Provisioning script completed!");

            _console.Information("Shutting down vm.");
            vm.Stop();

            vm.NewSnapshot("Template");

            //To give vmware enough time to close properly and delete temp files.
            _thread.Sleep(60);

            _console.Information("Cleaning up VMX for template...");

            CleanUpVMX(template, templateFolder, vmxpath);

            _console.Information("Removing cache and log files...");
            if (_directory.Exists($"{templateFolder}\\caches"))
                _directory.Delete($"{templateFolder}\\caches", true);

            foreach(var log in _directory.GetFiles(templateFolder, "*.log"))
                _file.Delete(log);

            if(_file.Exists($"{templateFolder}\\nvram"))
                _file.Delete($"{templateFolder}\\nvram");
            
            _console.Information("Compressing template");
            _compressHelper.CreateFromDirectory(templateFolder, $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate", CompressionLevel.Optimal, false, Encoding.UTF8, f => !f.EndsWith(".lck"));

            _console.Information("Template compressed and stored at {template}", $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate");

            _console.Information("Clearing up generated files.");
            _directory.Delete(templateFolder, true);

        }

        private void ProvisionVM(GraphModels.VM vm, string vmxpath)
        {
            var vmx = _vmxFactory();
            vmx.ReadFromFile(vmxpath);

            vmx.WriteValue("displayName", vm.Name);
            vmx.WriteValue("memsize", vm.Memeory.ToString());
            vmx.WriteValue("numvcpus", (vm.CPUCores * vm.CPUs).ToString());
            vmx.WriteValue("cpuid.coresPerSocket", vm.CPUCores.ToString());

            ProvisionNetwork(vm.Networks, GuestOS.Windows10, vmx);

            vmx.WriteToFile(vmxpath);
        }

        public bool TemplateExist(string name)
        {
            var templatePath = $"{_config.GetSetting("TemplateDir")}\\{name}";

            return _directory.Exists(templatePath);
        }

        public void BuildVMFromTemplate(GraphModels.VM vm)
        {
            var templatePath = $"{_config.GetSetting("TemplateDir")}\\{vm.Template}";
            var id = Guid.NewGuid();
            var vmFolder = $"{_environment.CurrentDirectory}\\_vmlab\\VMs\\{id}\\{vm.Name}";

            _directory.CreateDirectory(vmFolder);
            

            var templateVM = _vix.ConnectToVM($"{templatePath}\\{vm.Template}.vmx");
            var snapshot = _vix.GetSnapshots(templateVM).First(s => _vix.GetSnapshotName(s) == "Template");

            _vix.Clone($"{vmFolder}\\{vm.Name}.vmx", templateVM, snapshot, true);
            _vix.CloseObject(snapshot);
            _vix.CloseObject(templateVM);

            ProvisionVM(vm, $"{vmFolder}\\{vm.Name}.vmx");

            var vmcontrol = _loader.GetVMFromPath($"{vmFolder}\\{vm.Name}.vmx");
            vmcontrol.Start();
            vm.OnProvision(vmcontrol);
        }

        public IVMControl GetVM(string name)
        {
            if (!_directory.Exists($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\"))
                return null;

            return (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                    where _file.Exists($"{dir}\\{name}\\{name}.vmx")
                    select _loader.GetVMFromPath($"{dir}\\{name}\\{name}.vmx")).FirstOrDefault();
        }

        public void ImportTemplate(string path)
        {
            var templateName = Path.GetFileNameWithoutExtension(path);

            if (_directory.Exists($"{_config.GetSetting("TemplateDir")}\\{templateName}"))
            {
                _console.Error("Template already exists!");
                return;
            }

            _directory.CreateDirectory($"{_config.GetSetting("TemplateDir")}\\{templateName}");

            _compressHelper.ExtractToFolder(path, $"{_config.GetSetting("TemplateDir")}\\{templateName}");
        }

        public void RemoveTemplate(string name)
        {
            if (_directory.Exists($"{_config.GetSetting("TemplateDir")}\\{name}"))
                _directory.Delete($"{_config.GetSetting("TemplateDir")}\\{name}", true);
            
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

        private void CleanUpVMX(Template template, string templateFolder, string vmxpath)
        {
            var vmx = _vmxFactory();

            vmx.ReadFromFile(vmxpath);

            vmx.ClearValue("sata0:0"); //Remove CDRom
            vmx.ClearValue("ethernet"); //Remove Networks
            vmx.ClearValue("floppy0"); //Removing floppy
            vmx.WriteValue("floppy0.present", "FALSE");
            vmx.WriteValue("uuid.action", "keep");

            vmx.WriteToFile(vmxpath);
        }

        private void GenerateVMFiles(Template template, string templateFolder, string vmxpath)
        {
            var vmx = _vmxFactory();

            _console.Information("Begin Building Template: {name}", template.Name);


            if (!_directory.Exists(templateFolder))
                _directory.CreateDirectory(templateFolder);

            vmx.WriteValue(".encoding", "windows-1252");
            vmx.WriteValue("config.version", "8");
            vmx.WriteValue("virtualHW.version", "12");
            vmx.WriteValue("memsize", template.Memory.ToString());
            vmx.WriteValue("mem.hotadd", "true");
            vmx.WriteValue("numvcpus", (template.CPUCores * template.CPUs).ToString());
            vmx.WriteValue("cpuid.coresPerSocket", template.CPUCores.ToString());
            vmx.WriteValue("scsi0.present", "TRUE");
            vmx.WriteValue("scsi0.virtualDev", "lsisas1068");
            vmx.WriteValue("sata0.present", "TRUE");
            vmx.WriteValue("displayName", template.Name);
            vmx.WriteValue("guestOS", _osTranslator.FromVMLabGuestOS(template.GuestOS, template.Arch));
            vmx.WriteValue("powerType.powerOff", "soft");
            vmx.WriteValue("powerType.powerOn", "soft");
            vmx.WriteValue("powerType.suspend", "soft");
            vmx.WriteValue("powerType.reset", "soft");
            vmx.WriteValue("sound.present", "TRUE");
            vmx.WriteValue("sound.virtualDev", "hdaudio");
            vmx.WriteValue("sound.fileName", "-1");
            vmx.WriteValue("sound.autodetect", "TRUE");
            vmx.WriteValue("pciBridge0.present", "TRUE");
            vmx.WriteValue("pciBridge4.present", "TRUE");
            vmx.WriteValue("pciBridge4.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge4.functions", "8");
            vmx.WriteValue("pciBridge5.present", "TRUE");
            vmx.WriteValue("pciBridge5.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge5.functions", "8");
            vmx.WriteValue("pciBridge6.present", "TRUE");
            vmx.WriteValue("pciBridge6.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge6.functions", "8");
            vmx.WriteValue("pciBridge7.present", "TRUE");
            vmx.WriteValue("pciBridge7.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge7.functions", "8");
            vmx.WriteValue("bios.bootOrder", "cdrom, hdd, floppy");

            var index = 0;

            if (template.ISO != null)
            {
                if (!_file.Exists(template.ISO.LocalPath))
                {
                    _console.Information("ISO not found locally! Downloading: {url}", template.ISO.URL);
                    _fileDownloader.DownloadFile(template.ISO.URL, template.ISO.LocalPath);
                }

                vmx.WriteValue("sata0:0.present", "TRUE");
                vmx.WriteValue("sata0:0.fileName", _environment.CurrentDirectory + "\\" + template.ISO.LocalPath);
                vmx.WriteValue("sata0:0.deviceType", "cdrom-image");
            }

            foreach (var disk in template.HardDisks)
            {
                vmx.WriteValue($"scsi0:{index}.present", "TRUE");
                vmx.WriteValue($"scsi0:{index}.fileName", $"disk{index}.vmdk");

                var sizeinBytes = (long)disk.Size * 1024 * 1024 * 1024; // Converting GiB into Bytes
                _driveBuilder.BuildDrive($"{templateFolder}\\disk{index}.vmdk", sizeinBytes);
                index++;
            }

            if (!string.IsNullOrEmpty(template.FloppyImage))
            {
                vmx.WriteValue("floppy0.fileType", "file");
                vmx.WriteValue("floppy0.fileName", "autoinst.flp");
                vmx.WriteValue("floppy0.clientDevice", "FALSE");

                _file.Copy(template.FloppyImage, $"{templateFolder}\\autoinst.flp");
            }

            if (template.Networks.Count <= 0) return;

            ProvisionNetwork(template.Networks, template.GuestOS, vmx);

            vmx.WriteToFile(vmxpath);
        }

        private void ProvisionNetwork(IEnumerable<Network> networks, GuestOS os, IVMXCollection vmx)
        {
            var index = 0;
            foreach (var nic in networks)
            {
                vmx.WriteValue($"ethernet{index}.present", "TRUE");

                switch (nic.Type)
                {
                    case NetworkType.Bridged:
                        vmx.WriteValue($"ethernet{index}.connectionType", "bridged");
                        break;
                    case NetworkType.NAT:
                        vmx.WriteValue($"ethernet{index}.connectionType", "nat");
                        break;
                    case NetworkType.Private:
                        vmx.WriteValue($"ethernet{index}.connectionType", "pvn");
                        vmx.WriteValue($"ethernet{index}.pvnID", _ipvnHelper.GetPVN(nic.Name));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (os == GuestOS.Windows7 || os == GuestOS.Windows2008R2 ||
                    os == GuestOS.Windows2008R2Core)
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000");
                else
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000e");

                vmx.WriteValue($"ethernet{index}.wakeOnPcktRcv", "FALSE");
                vmx.WriteValue($"ethernet{index}.addressType", "generated");

                index++;
            }
        }
    }
}
