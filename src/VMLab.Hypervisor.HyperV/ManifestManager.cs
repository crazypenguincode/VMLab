using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using Newtonsoft.Json;
using Serilog;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.Contract.Helpers;
using VMLab.GraphModels;
using VMLab.Helper;

namespace VMLab.Hypervisor.HyperV
{
    public class ManifestManagerSingleton : IManifestManager
    {
        private readonly ICompressHelper _compressHelper;
        private readonly IConfig _config;
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly ILogger _log;
        private readonly IEnvironment _environment;

        public ManifestManagerSingleton(ICompressHelper compressHelper, IConfig config, IDirectory directory, IFile file, ILogger log, IEnvironment environment)
        {
            _compressHelper = compressHelper;
            _config = config;
            _directory = directory;
            _file = file;
            _log = log;
            _environment = environment;
        }

        public TemplateManifest GetTemplateManifestFromArchive(string path)
        {
            return JsonConvert.DeserializeObject<TemplateManifest>(_compressHelper.GetTextFromZip(path, "manifest.json")
                .LogWithObject(o => _log.Information("Getting manifest from archive {path}. {@manifest}", path, o)));
        }

        public IEnumerable<TemplateManifest> GetInstalledTemplateManifests()
        {
            var templatedir = _config.GetSetting("TemplateDir");

            return _directory.GetFiles(templatedir, "manifest.json", SearchOption.AllDirectories).Select(file =>
                {
                    var manifest = JsonConvert.DeserializeObject<TemplateManifest>(_file.ReadAllText(file));
                    manifest.Path = Path.GetDirectoryName(file);

                    return manifest;
                })
                .Where(m => m.Hypervisor == "Hyperv")
                .ToList()
                .LogWithObject(o => _log.Information("Manifests: {@manifests}", o));
        }

        public TemplateManifest GetManifestFromVM(VM vm)
        {
            var vmfolder = (from dir in _directory.GetDirectories($"{_environment.CurrentDirectory}\\_vmlab\\VMs\\")
                where _file.Exists($"{dir}\\{vm.Name}\\manifest.json")
                select $"{dir}\\{vm.Name}").FirstOrDefault();

            return vmfolder == null ? null : FromFile($"{vmfolder}\\manifest.json");
        }

        public TemplateManifest FromFile(string path)
        {
            if (_file.Exists(path))
                return JsonConvert.DeserializeObject<TemplateManifest>(_file.ReadAllText(path))
                    .LogWithObject(o => _log.Information("Manifests: {@manifests}", o));

            return null;
        }
    }
}
