using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.GraphModels;

namespace VMLab.Script.FluentInterface
{
    public interface ISession
    {
        IEnumerable<IVMControl> VMs { get; }
        string LabName { get; }
        string LabAuthor { get; }
        string LabDescription { get; }
        
    }
}
