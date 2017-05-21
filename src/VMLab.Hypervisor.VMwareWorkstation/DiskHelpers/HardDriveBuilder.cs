using DiscUtils.Vmdk;

namespace VMLab.Hypervisor.VMwareWorkstation.DiskHelpers
{
    public class HardDriveBuilder : IHardDriveBuilder
    {
        public void BuildDrive(string path, long size)
        {
            using (Disk.Initialize(path, new DiskParameters
            {
                AdapterType = DiskAdapterType.LsiLogicScsi,
                Capacity = size,
                CreateType = DiskCreateType.MonolithicSparse
            }))
            {
                
            }
        }
    }
}
