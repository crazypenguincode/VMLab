using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Core.hypervisorDetectors
{
    public interface IHypervisorDetector
    {
        string Name { get; }
        bool Detected();
    }
}
