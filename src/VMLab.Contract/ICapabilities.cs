using System.Collections.Generic;
using System.Data.Common;
using VMLab.GraphModels;

namespace VMLab.Contract
{
    public interface ICapabilities
    {
        string Hypervisor { get; }
        IEnumerable<GuestOS> SupportedGuestOS { get; }
        IEnumerable<Arch>  SupportedArch { get; }
        IEnumerable<string> HypervisorCapabilities { get; }
    }
}
