using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.GraphModels
{
    public enum DiskBus
    {
        IDE,
        SCSI,
        SATA
    }

    public class HardDisk
    {
        private long Size { get; set; }
        public DiskBus Bus { get; set; }
    }
}
