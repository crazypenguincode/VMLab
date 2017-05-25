using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Helper
{
    public interface IHypervisorFinder
    {
        string[] Hypervisors { get; }
    }
}
