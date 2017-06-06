using SystemInterface.IO;

namespace VMLab.Core.hypervisorDetectors
{
    public class VMwareWorkstationDetector : IHypervisorDetector
    {
        public string Name => "VMwareworkstation";
        private readonly IFile _file;

        public VMwareWorkstationDetector(IFile file)
        {
            _file = file;
        }

        public bool Detected()
        {
            return _file.Exists("C:\\Program Files (x86)\\VMware\\VMware Workstation\\vmware.exe");
        }
    }
}
