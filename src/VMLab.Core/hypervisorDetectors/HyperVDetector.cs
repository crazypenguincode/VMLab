using System.Linq;
using System.Management.Automation;

namespace VMLab.Core.hypervisorDetectors
{
    public class HyperVDetectorSingleton : IHypervisorDetector
    {
        public string Name => "HyperV";
        public bool Detected()
        {
            try
            {
                var powerShell = PowerShell.Create();
                powerShell.AddScript("(Get-WindowsOptionalFeature -FeatureName Microsoft-Hyper-V-All -Online).State");

                return powerShell.Invoke<string>().FirstOrDefault() == "Enabled";
            }
            catch
            {
                return false;
            }
        }
    }
}
