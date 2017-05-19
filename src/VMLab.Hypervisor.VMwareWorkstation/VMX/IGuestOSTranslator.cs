using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public interface IGuestOSTranslator
    {
        GuestOS FromVMGuestOS(string guestos);
        string FromVMLabGuestOS(GuestOS os, Arch arch);
    }
}
