using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV
{
    public class VMManager : IVMManager
    {
        public IVMControl GetVM(VM vm)
        {
            throw new NotImplementedException();
        }

        public void DestroyVM(VM vm, IVMControl control)
        {
            throw new NotImplementedException();
        }

        public void PreStart(VM vm)
        {
            throw new NotImplementedException();
        }

        public void PostStart(IVMControl control, VM vm)
        {
            throw new NotImplementedException();
        }
    }
}
