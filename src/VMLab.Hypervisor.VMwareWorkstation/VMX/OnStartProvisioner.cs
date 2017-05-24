using System;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public class OnStartProvisioner : IOnStartProvisioner
    {
        private readonly IPVNHelper _pvnHelper;
        private readonly IManifestManager _manifestManager;

        public OnStartProvisioner(IPVNHelper pvnHelper, IManifestManager manifestManager)
        {
            _pvnHelper = pvnHelper;
            _manifestManager = manifestManager;
        }


        public void PreStart(IVMXCollection vmx, GraphModels.VM vm)
        {
            var manifest = _manifestManager.GetManifestFromVM(vm);

            //strip shared folders.
            vmx.ClearValue("sharedFolder");

            var index = 0;
            
            foreach (var nic in vm.Networks)
            {

                //Do not clear existing ethernet settings to keep the same mac address.

                vmx.WriteValue($"ethernet{index}.present", "TRUE");

                switch (nic.Type)
                {
                    case NetworkType.Bridged:
                        vmx.WriteValue($"ethernet{index}.connectionType", "bridged");
                        break;
                    case NetworkType.NAT:
                        vmx.WriteValue($"ethernet{index}.connectionType", "nat");
                        break;
                    case NetworkType.Private:
                        vmx.WriteValue($"ethernet{index}.connectionType", "pvn");
                        vmx.WriteValue($"ethernet{index}.pvnID", _pvnHelper.GetPVN(nic.Name));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (manifest.OS == GuestOS.Windows7 || manifest.OS == GuestOS.Windows2008R2 ||
                    manifest.OS == GuestOS.Windows2008R2Core)
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000");
                else
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000e");

                vmx.WriteValue($"ethernet{index}.wakeOnPcktRcv", "FALSE");
                vmx.WriteValue($"ethernet{index}.addressType", "generated");

                index++;
            }

            vmx.WriteValue("displayName", vm.Name);
            vmx.WriteValue("memsize", vm.Memeory.ToString());
            vmx.WriteValue("numvcpus", (vm.CPUCores * vm.CPUs).ToString());
            vmx.WriteValue("cpuid.coresPerSocket", vm.CPUCores.ToString());

            foreach (var settings in vm.Properties)
                vmx.WriteValue(settings.Key, settings.Value);

            if (vm.NestedVirtualization)
            {
                vmx.WriteValue("hypervisor.cpuid.v0", "FALSE");
                vmx.WriteValue("mce.enable", "TRUE");
                vmx.WriteValue("vhv.enable", "TRUE");
            }
            else
            {
                vmx.ClearValue("hypervisor.cpuid.v0");
                vmx.ClearValue("mce.enable");
                vmx.ClearValue("vhv.enable");
            }
            
        }

        public void PostStart(IVMControl vm, GraphModels.VM model)
        {
            var index = 0;
            foreach (var share in model.SharedFolders)
            {
                vm.Exec("c:\\windows\\system32\\cmd.exe", $"/c rd /s /q \"{share.GuestPath}\"", false);
                vm.AddSharedFolder($"share{index}", share.HostPath, share.GuestPath);
                index++;
            }
        }
    }
}
