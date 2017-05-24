using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract
{
    public interface IVMManager
    {
        IVMControl GetVM(VM vm);
        void DestroyVM(VM vm, IVMControl control);
        void PreStart(VM vm);
        void PostStart(IVMControl control, VM vm);
    }
}
