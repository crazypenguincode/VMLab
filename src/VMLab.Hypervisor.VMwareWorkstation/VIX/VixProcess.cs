using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Hypervisor.VMwareWorkstation.VIX
{
    public class VixProcess
    {
        public string Name { get; set; }
        public long ProcessID { get; set; }
        public string Owner { get; set; }
        public string Command { get; set; }
    }
}
