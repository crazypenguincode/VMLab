using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public interface IOnStartProvisioner
    {
        void PreStart(IVMXCollection vmx, GraphModels.VM vm);
        void PostStart(IVMControl vm, GraphModels.VM model);
    }
}
