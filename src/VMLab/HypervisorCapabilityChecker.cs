using System;
using System.Linq;
using VMLab.Contract;
using VMLab.GraphModels;

namespace VMLab
{
    public class HypervisorCapabilityChecker : IHypervisorCapabilityChecker
    {

        private readonly ICapabilities _capabilities;
        public HypervisorCapabilityChecker(ICapabilities capabilities)
        {
            _capabilities = capabilities;
        }

        public Tuple<bool, string> CheckTemplate(Template template)
        {
            if (!_capabilities.SupportedGuestOS.Contains(template.GuestOS))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support this guest operating system!");

            if(!_capabilities.SupportedArch.Contains(template.Arch))
                return new Tuple<bool, string>(false, "Hypervisor does not support target architecture!");

            if(!string.IsNullOrEmpty(template.FloppyImage) && !_capabilities.HypervisorCapabilities.Contains("VM_FLOPPYIMAGE"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support floppy disk images.");

            if(template.HardDisks.Count > 0 && !_capabilities.HypervisorCapabilities.Contains("VM_HDDISK"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support hard disks!");

            if(template.ISO != null && !_capabilities.HypervisorCapabilities.Contains("VM_DVDISO"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support ISO files!");

            if(template.Networks.Count > 0 && !_capabilities.HypervisorCapabilities.Contains("VM_NETWORK"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support Networking!");

            if(template.Networks.Any(n => n.Type == NetworkType.Bridged) && !_capabilities.HypervisorCapabilities.Contains("VM_NETWORK_BRIDGED"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support Bridged networks!");

            if (template.Networks.Any(n => n.Type == NetworkType.Private) && !_capabilities.HypervisorCapabilities.Contains("VM_NETWORK_PRIVATE"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support Private networks!");

            if (template.Networks.Any(n => n.Type == NetworkType.NAT) && !_capabilities.HypervisorCapabilities.Contains("VM_NETWORK_NAT"))
                return new Tuple<bool, string>(false, "Hypervisor doesn't support Nat networks!");

            return new Tuple<bool, string>(true, "");
        }
    }
}