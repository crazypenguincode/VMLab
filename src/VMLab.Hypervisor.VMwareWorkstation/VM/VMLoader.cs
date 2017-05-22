using System;
using System.Collections.Generic;
using VMLab.GraphModels;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VM
{
    public class VMLoader : IVMLoader
    {
        private readonly Func<IVMControl> _vmcontrolFactory;

        public VMLoader(Func<IVMControl> vmcontrolFactory)
        {
            _vmcontrolFactory = vmcontrolFactory;
        }

        public IVMControl GetVMFromPath(string vmx, IEnumerable<Credential> creds)
        {
            var vm = _vmcontrolFactory() as VMControl;
            vm.SetVMXFile(vmx);
            vm.SetCredentials(creds);
            vm.SetCredentials("admin");
            
            return vm;
        }
    }
}
