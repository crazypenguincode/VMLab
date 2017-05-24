using System.Collections.Generic;
using VMLab.Contract.GraphModels;

namespace VMLab.Contract
{
    public interface IManifestManager
    {
        TemplateManifest GetTemplateManifestFromArchive(string path);
        IEnumerable<TemplateManifest> GetInstalledTemplateManifests();
    }
}
