using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SystemInterface;
using SystemInterface.IO;
using Newtonsoft.Json;
using Serilog;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.Contract.Helpers;
using VMLab.Contract.SemVer;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Hypervisor.HyperV.HyperV;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.HyperV
{
    public class TemplateManager : ITemplateManager
    {
        private readonly ILogger _log;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IHyperV _hyperV;
        private readonly IVMLoader _vmLoader;
        private readonly IConsole _console;
        private readonly ICompressHelper _compressHelper;
        private readonly IEnvironment _environment;
        private readonly IPath _path;
        private readonly IManifestManager _manifestManager;
        private readonly IConfig _config;
        private readonly IFileDownloader _fileDownloader;

        public TemplateManager(ILogger log, IFile file, IDirectory directory, IHyperV hyperV, IVMLoader vmLoader, IConsole console, ICompressHelper compressHelper, IPath path, IEnvironment environment, IManifestManager manifestManager, IConfig config, IFileDownloader fileDownloader)
        {
            _log = log;
            _file = file;
            _directory = directory;
            _hyperV = hyperV;
            _vmLoader = vmLoader;
            _console = console;
            _compressHelper = compressHelper;
            _path = path;
            _environment = environment;
            _manifestManager = manifestManager;
            _config = config;
            _fileDownloader = fileDownloader;
        }

        public bool CanBuild(Template template)
        {
            if (template.CPUCores > 0)
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

            _hyperV.NewVM(template.Name, templateFolder);
            _hyperV.SetCPUCount(template.Name, template.CPUs);
            _hyperV.SetMemory(template.Name, template.Memory, false, 0, 0);

            if (template.ISO != null)
            {
                if (!_file.Exists(template.ISO.LocalPath))
                {
                    if (string.IsNullOrEmpty(template.ISO.URL))
                    {
                        _console.Information("You have not specified a URL to download the ISO from and it doesn't exist in the file system!");
                        throw new ApplicationException("Failed to find iso file!");
                    }

                    _console.Information("ISO not found locally! Downloading: {url}", template.ISO.URL);
                    _fileDownloader.DownloadFile(template.ISO.URL, template.ISO.LocalPath);
                }

                 _hyperV.AddISO(template.Name, _environment.CurrentDirectory + "\\" + template.ISO.LocalPath);
            }

            var index = 0;

            foreach (var disk in template.HardDisks)
            {
                index++;
                _hyperV.AddDisk(template.Name, $"{templateFolder}\\Virtual Hard Disks\\{template.Name}{index}.vhdx", disk.Size, index);
            }

            if (!string.IsNullOrEmpty(template.FloppyImage))
                _hyperV.SetFloppyImage(template.Name, template.FloppyImage);

            foreach (var network in template.Networks)
            {
                _hyperV.AddNetwork(template.Name, network.Name);
            }

            var vm = _vmLoader.LoadByName(template.Name, templateFolder, template: template);
            vm.SetCredentials("Admin");

            vm.Start(false);

            if(!template.HeadLess)
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

            _directory.CreateDirectory(templateFolder + "\\Template");

            foreach (var disk in _directory.GetFiles(templateFolder, "*.vhdx", SearchOption.AllDirectories))
            {
                _file.Move(disk, templateFolder + "\\Template\\" + _path.GetFileName(disk));
            }

            _file.Move($"{templateFolder}\\manifest.json", templateFolder + "\\Template\\manifest.json");

            _console.Information("Compressing template");
            _compressHelper.CreateFromDirectory(templateFolder + "\\Template", $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate");
            _console.Information("Template compressed and stored at {template}", $"{_environment.CurrentDirectory}\\{template.Name}.vmlabtemplate");

            _console.Information("Clearing up generated files.");
            _directory.Delete(templateFolder, true);
            _log.Information("Template built");
        }

        public void BuildVMFromTemplate(VM vm)
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

            _directory.CreateDirectory(vmFolder);

            _hyperV.NewVM(vm.Name, vmFolder);
            _hyperV.SetCPUCount(vm.Name, vm.Memeory);
            _hyperV.SetMemory(vm.Name, vm.Memeory, false, vm.Memeory, vm.Memeory);

            var index = 0;
            //Create difference disks
            foreach (var disk in _directory.GetFiles(templatePath, "*.vhdx", SearchOption.AllDirectories))
            {
                index++;
                _hyperV.CreateDifferenceDisk(disk, vmFolder + "\\" + _path.GetFileName(disk));
                _hyperV.AddDisk(vm.Name, vmFolder + "\\" + _path.GetFileName(disk), index);
            }

            _file.Copy(templatePath + "\\manifest.json", vmFolder + "\\manifest.json");

            var control = _vmLoader.LoadByName(vm.Name, vmFolder, model: vm);
            control.Start();

            vm.OnProvision(control);

            _log.Information("VM build!");   
        }

        public void ImportTemplate(string path)
        {
            _log.Information("Importing template {name}", path);
            var data = _compressHelper.GetTextFromZip(path, "manifest.json");
            var manifest = JsonConvert.DeserializeObject<TemplateManifest>(data);
            var templateDir = $"{_config.GetSetting("TemplateDir")}\\HyperV\\{manifest.Name}_{manifest.Version}";

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
    }
}
