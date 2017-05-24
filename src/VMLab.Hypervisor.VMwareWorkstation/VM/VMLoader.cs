using System;
using System.Collections.Generic;
using System.IO;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VM
{
    public class VMLoader : IVMLoader
    {
        private readonly Func<IVMControl> _vmcontrolFactory;
        private readonly IManifestManager _manifestManager;

        public VMLoader(Func<IVMControl> vmcontrolFactory, IManifestManager manifestManager)
        {
            _vmcontrolFactory = vmcontrolFactory;
            _manifestManager = manifestManager;
        }

        public IVMControl GetVMFromPath(string vmx, IEnumerable<Credential> creds, GraphModels.VM model = null)
        {
            var manifest = _manifestManager.FromFile($"{Path.GetDirectoryName(vmx)}\\manifest.json");
            var vm = _vmcontrolFactory() as VMControl;

            if (vm == default(VMControl))
                return null;

            vm.SetVMXFile(vmx, manifest, model);
            vm.SetCredentials(creds);
            vm.SetCredentials("admin");
            
            return vm;
        }
    }
}
