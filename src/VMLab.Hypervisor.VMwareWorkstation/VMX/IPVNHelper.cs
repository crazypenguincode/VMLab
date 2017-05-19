using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public interface IPVNHelper
    {
        string GetPVN(string name);
    }
}
