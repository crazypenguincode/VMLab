using System.Collections.Generic;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV.HyperV
{
    public interface IVMLoader
    {
        IVMControl LoadByName(string name, string path, IEnumerable<Credential> creds = null, VM model = null,
            Template template = null);
    }
}
