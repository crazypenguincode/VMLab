using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.Null
{
    public class ManifestManager : IManifestManager
    {
        public TemplateManifest GetTemplateManifestFromArchive(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TemplateManifest> GetInstalledTemplateManifests()
        {
            throw new NotImplementedException();
        }

        public TemplateManifest GetManifestFromVM(VM vm)
        {
            throw new NotImplementedException();
        }

        public TemplateManifest FromFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
