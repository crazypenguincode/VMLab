using DiscUtils.Vmdk;
using Serilog;

namespace VMLab.Hypervisor.VMwareWorkstation.DiskHelpers
{
    public class HardDriveBuilder : IHardDriveBuilder
    {
        private readonly ILogger _log;

        public HardDriveBuilder(ILogger log)
        {
            _log = log;
        }

        public void BuildDrive(string path, long size)
        {
            _log.Information("Creating VMDK file at {file} with capacity of {capacity}", path, size);
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
