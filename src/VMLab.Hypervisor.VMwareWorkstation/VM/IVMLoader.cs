using System.Collections.Generic;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public interface IVMLoader
    {
        IVMControl GetVMFromPath(string vmx, IEnumerable<Credential> creds = null, GraphModels.VM model = null, Template template = null);
    }
}
