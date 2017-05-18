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
        //IEnumerable<IAction> Actions { get; }
        IEnumerable<ITemplate> Templates { get; }
        IDictionary<string, string> Properties { get; }
        string LabName { get; }
        string LabAuthor { get; }
        string LabDescription { get; }
        
    }
}
