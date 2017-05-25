using System.Linq;
using VMLab.GraphModels;

namespace VMLab.Contract.OSEnvironment
{
    public class OSEnvironmentManager : IOSEnvironmentManager
    {
        private readonly IOSEnvironment[] _environments;

        public OSEnvironmentManager(IOSEnvironment[] environments)
        {
            _environments = environments;
        }

        public IOSEnvironment GetOSEnvironment(GuestOS os, Arch arch)
        {
            return _environments.Where(o => o.SupportedOS.Contains(os) && o.SupportedArch.Contains(arch))
                .OrderBy(o => o.Priority)
                .FirstOrDefault();
        }
    }
}
