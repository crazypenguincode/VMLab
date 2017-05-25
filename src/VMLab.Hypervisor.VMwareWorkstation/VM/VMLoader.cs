using System;
using System.Collections.Generic;
using System.IO;
using VMLab.Contract;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VM
{
    public class VMLoader : IVMLoader
    {
        private readonly Func<IVMControl> _vmcontrolFactory;
        private readonly IManifestManager _manifestManager;
        private readonly ICredentialManager _credentialManager;

        public VMLoader(Func<IVMControl> vmcontrolFactory, IManifestManager manifestManager, ICredentialManager credentialManager)
        {
            _vmcontrolFactory = vmcontrolFactory;
            _manifestManager = manifestManager;
            _credentialManager = credentialManager;
        }

        public IVMControl GetVMFromPath(string vmx, IEnumerable<Credential> creds = null, GraphModels.VM model = null, Template template=null)
        {
            var manifest = _manifestManager.FromFile($"{Path.GetDirectoryName(vmx)}\\manifest.json");
            var vm = _vmcontrolFactory() as VMControl;

            if (vm == default(VMControl))
                return null;

            var usedCredentials = default(IEnumerable<Credential>);

            if (creds != null)
                usedCredentials = creds;
            if (model != null)
                usedCredentials = _credentialManager.AllCredentials(model);
            if (template != null)
                usedCredentials = _credentialManager.AllCredentials(template);

            vm.SetVMXFile(vmx, manifest, model);
            vm.SetCredentials(usedCredentials);
            vm.SetCredentials("admin");

            if(model != null)
                _credentialManager.LoadSecureCredentials(model);
            
            return vm;
        }
    }
}
