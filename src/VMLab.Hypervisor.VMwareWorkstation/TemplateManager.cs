using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Threading;
using Newtonsoft.Json;
using Serilog;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.Contract.Helpers;
using VMLab.Contract.SemVer;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Hypervisor.VMwareWorkstation.DiskHelpers;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Hypervisor.VMwareWorkstation.VM;
using VMLab.Hypervisor.VMwareWorkstation.VMX;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class TemplateManager : ITemplateManager
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
        private readonly IManifestManager _manifestManager;
        private readonly ILogger _log;
        private readonly IOnStartProvisioner _onStartProvisioner;

        public TemplateManager(Func<IVMXCollection> vmxFactory, IHardDriveBuilder driveBuilder, IFile file, IDirectory directory, IPVNHelper ipvnHelper, IGuestOSTranslator osTranslator, IEnvironment environment, IFileDownloader fileDownloader, IConsole console, IVMLoader loader, ICompressHelper compressHelper, IThread thread, IConfig config, IVIX vix, IManifestManager manifestManager, ILogger log, IOnStartProvisioner onStartProvisioner)
        {
            _vmxFactory = vmxFactory;
            _driveBuilder = driveBuilder;
            _file = file;
            _directory = directory;
            _ipvnHelper = ipvnHelper;
            _osTranslator = osTranslator;
            _environment = environment;
            _fileDownloader = fileDownloader;
            _console = console;
            _loader = loader;
            _compressHelper = compressHelper;
            _thread = thread;
            _config = config;
            _vix = vix;
            _manifestManager = manifestManager;
            _log = log;
            _onStartProvisioner = onStartProvisioner;
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
            _log.Information("Building template at {folder} template: {@template}", templateFolder, template);

            //Building manfiest
            var manifest = new TemplateManifest
            {
                Name = template.Name,
                Hypervisor = "Vmwareworkstation",
                OS = template.GuestOS,
                Arch = template.Arch,
                Version = template.Version
            };

            if (!_directory.Exists(templateFolder))
                _directory.CreateDirectory(templateFolder);

            _file.WriteAllText($"{templateFolder}\\manifest.json", JsonConvert.SerializeObject(manifest));

            var vmxpath = $"{templateFolder}\\{template.Name}.vmx";

            GenerateVMFiles(template, templateFolder, vmxpath);

            var vm = _loader.GetVMFromPath(vmxpath, template: template) as VMControl;

            if (vm == null)
                throw new NullReferenceException("Can't load VM controller object!");

            vm.SetCredentials("Admin");

            vm.Start(false);

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

            CleanUpVMX(vmxpath);

            _console.Information("Removing cache and log files...");
            if (_directory.Exists($"{templateFolder}\\caches"))
                _directory.Delete($"{templateFolder}\\caches", true);

            foreach (var log in _directory.GetFiles(templateFolder, "*.log"))
                _file.Delete(log);

            if (_file.Exists($"{templateFolder}\\nvram"))
                _file.Delete($"{templateFolder}\\nvram");

            _console.Information("Compressing template");
            _compressHelper.CreateFromDirectory(templateFolder, $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate", CompressionLevel.Optimal, false, Encoding.UTF8, f => !f.EndsWith(".lck"));

            _console.Information("Template compressed and stored at {template}", $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate");

            _console.Information("Clearing up generated files.");
            _directory.Delete(templateFolder, true);
            _log.Information("Template built");
        }

        public void BuildVMFromTemplate(GraphModels.VM vm)
        {
            _log.Information("Building VM from template {@vm}", vm);
            var manifest = default(TemplateManifest);

            if (vm.Version == "latest")
            {
                manifest = _manifestManager.GetInstalledTemplateManifests()
                    .Where(m => m.Name.ToLower() == vm.Template.ToLower())
                    .Where(m => Regex.IsMatch(m.Version, "^[0-9]{1,5}\\.[0-9]{1,5}\\.[0-9]{1,5}$")) //Remove prerelease versions.
                    .OrderByDescending(m => new SemVer(m.Version)) //Sort by versions.
                    .FirstOrDefault();
            }
            else
            {
                manifest = _manifestManager.GetInstalledTemplateManifests()
                    .FirstOrDefault(m => m.Name == vm.Template && m.Version == vm.Version);
            }

            if (manifest == default(TemplateManifest))
            {
                _console.Error("Can't find template {name} version: {version}", vm.Template, vm.Version);
                return;
            }

            var templatePath = manifest.Path;
            var id = Guid.NewGuid();
            var vmFolder = $"{_environment.CurrentDirectory}\\_vmlab\\VMs\\{id}\\{vm.Name}";
            var vmxPath = $"{vmFolder}\\{vm.Name}.vmx";

            _directory.CreateDirectory(vmFolder);

            var templateVM = _vix.ConnectToVM($"{templatePath}\\{vm.Template}.vmx");
            var snapshot = _vix.GetSnapshots(templateVM).First(s => _vix.GetSnapshotName(s) == "Template");

            _vix.Clone($"{vmFolder}\\{vm.Name}.vmx", templateVM, snapshot, true);
            _vix.CloseObject(snapshot);
            _vix.CloseObject(templateVM);

            _file.Copy($"{manifest.Path}\\manifest.json", $"{vmFolder}\\manifest.json");



            var vmcontrol = _loader.GetVMFromPath($"{vmFolder}\\{vm.Name}.vmx", model: vm);
            var vmx = _vmxFactory();

            vmx.ReadFromFile(vmxPath);
            _onStartProvisioner.PreStart(vmx, vm);
            vmx.WriteToFile(vmxPath);
            vmcontrol.Start(false);

            vmcontrol.WaitFile("c:\\provision.wait", false);
            _onStartProvisioner.PostStart(vmcontrol, vm);
            vm.OnProvision(vmcontrol);

            _log.Information("VM built!");
        }

        public void ImportTemplate(string path)
        {
            _log.Information("Importing template {name}", path);
            var data = _compressHelper.GetTextFromZip(path, "manifest.json");
            var manifest = JsonConvert.DeserializeObject<TemplateManifest>(data);
            var templateDir = $"{_config.GetSetting("TemplateDir")}\\Vmwareworkstation\\{manifest.Name}_{manifest.Version}";

            if (_directory.Exists(templateDir))
            {
                _console.Error("Template already exists!");
                return;
            }

            _directory.CreateDirectory(templateDir);

            _compressHelper.ExtractToFolder(path, templateDir);
        }

        public void RemoveTemplate(string name)
        {
            _log.Information("Removing template {name}", name);
            var manifest = _manifestManager.GetInstalledTemplateManifests().FirstOrDefault(m => m.Name == name);

            if (_directory.Exists(manifest?.Path))
                _directory.Delete(manifest?.Path, true);
        }
        private void CleanUpVMX(string vmxpath)
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
            vmx.WriteValue("gui.applyHostDisplayScalingToGuest", "FALSE");

            var index = 0;

            if (template.ISO != null)
            {
                if (!_file.Exists(template.ISO.LocalPath))
                {
                    if (String.IsNullOrEmpty(template.ISO.URL))
                    {
                        _console.Information("You have not specified a URL to download the ISO from and it doesn't exist in the file system!");
                        throw new ApplicationException("Failed to find iso file!");
                    }

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
