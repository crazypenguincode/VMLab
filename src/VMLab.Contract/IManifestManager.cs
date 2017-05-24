using System.Collections.Generic;
using VMLab.Contract.GraphModels;
using VMLab.GraphModels;

namespace VMLab.Contract
{
    public interface IManifestManager
    {
        TemplateManifest GetTemplateManifestFromArchive(string path);
        IEnumerable<TemplateManifest> GetInstalledTemplateManifests();
        TemplateManifest GetManifestFromVM(VM vm);
        TemplateManifest FromFile(string path);
    }
}
