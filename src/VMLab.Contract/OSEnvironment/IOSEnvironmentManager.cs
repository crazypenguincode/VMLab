using VMLab.GraphModels;

namespace VMLab.Contract.OSEnvironment
{
    public interface IOSEnvironmentManager
    {
        IOSEnvironment GetOSEnvironment(GuestOS os, Arch arch);
    }
}
