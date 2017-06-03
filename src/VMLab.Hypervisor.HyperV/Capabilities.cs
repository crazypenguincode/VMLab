using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.HyperV
{
    public class Capabilities : ICapabilities
    {
        public string Hypervisor { get; }
        public IEnumerable<GuestOS> SupportedGuestOS { get; }
        public IEnumerable<Arch> SupportedArch { get; }
        public IEnumerable<string> HypervisorCapabilities { get; }
    }
}
