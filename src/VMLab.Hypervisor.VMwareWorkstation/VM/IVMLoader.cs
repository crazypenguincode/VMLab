using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public interface IVMLoader
    {
        IVMControl GetVMFromPath(string vmx);
    }
}
