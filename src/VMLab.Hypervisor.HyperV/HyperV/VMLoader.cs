using System;
using System.Collections.Generic;
using VMLab.Contract;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV.HyperV
{
    public class VMLoader : IVMLoader
    {
        private readonly Func<IVMControl> _vmfactory;
        private readonly IManifestManager _manifestManager;
        private readonly ICredentialManager _credentialManager;

        public VMLoader(Func<IVMControl> vmfactory, IManifestManager manifestManager, ICredentialManager credentialManager)
        {
            _vmfactory = vmfactory;
            _manifestManager = manifestManager;
            _credentialManager = credentialManager;
        }

        public IVMControl LoadByName(string name, string path, IEnumerable<Credential> creds = null, VM model = null, Template template = null)
        {
            var manifest = _manifestManager.FromFile($"{path}\\manifest.json");
            var useCreds = default(IEnumerable<Credential>);

            if (creds != null)
                useCreds = creds;
            if (model != null)
                useCreds = _credentialManager.AllCredentials(model);
            if (template != null)
                useCreds = _credentialManager.AllCredentials(template);

            var control = (VMControl)_vmfactory();
            control.SetVMData(name, manifest, model, useCreds);
            control.SetCredentials("admin");
            
            return control;
        }
    }
}
