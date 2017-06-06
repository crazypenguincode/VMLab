using System.Collections.Generic;
using VMLab.Contract;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.HyperV
{
    public class Capabilities : ICapabilities
    {
        public string Hypervisor => "Hyperv";
        public IEnumerable<GuestOS> SupportedGuestOS => new[] { GuestOS.Windows10, GuestOS.Windows2016, GuestOS.Windows2016Core };
        public IEnumerable<Arch> SupportedArch => new[] {Arch.X64, Arch.X86};

        public IEnumerable<string> HypervisorCapabilities => new[]
        {
            "VM_POWER_ON",
            "VM_POWER_OFF",
            "VM_FORCEPOWER_OFF",
            "VM_RESET",
            "VM_FORCERESET",
            "VM_FLOPPYIMAGE",
            "VM_HDDISK",
            "VM_DVDISO",
            "VM_NETWORK",
            "VM_NETWORK_BRIDGED",
            "VM_NETWORK_PRIVATE",
            "VM_NETWORK_NAT"
        };
    }
}
