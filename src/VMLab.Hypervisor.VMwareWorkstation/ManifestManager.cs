using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using Newtonsoft.Json;
using Serilog;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.Contract.Helpers;
using VMLab.Helper;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class ManifestManager : IManifestManager
    {
        private readonly ICompressHelper _compressHelper;
        private readonly IConfig _config;
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly ILogger _log;

        public ManifestManager(ICompressHelper compressHelper, IConfig config, IDirectory directory, IFile file, ILogger log)
        {
            _compressHelper = compressHelper;
            _config = config;
            _directory = directory;
            _file = file;
            _log = log;
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
                .Where(m => m.Hypervisor == "Vmwareworkstation")
                .ToList()
                .LogWithObject(o => _log.Information("Manifests: {@manifests}", o));
        }
    }
}
