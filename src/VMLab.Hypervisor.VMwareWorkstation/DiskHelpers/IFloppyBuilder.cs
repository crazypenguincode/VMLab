using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.VMwareWorkstation.DiskHelpers
{
    public interface IFloppyBuilder
    {
        void Build(string path, IEnumerable<FloppyFile> files);
    }
}
