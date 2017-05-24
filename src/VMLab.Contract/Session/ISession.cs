using System.Collections.Generic;
using VMLab.Contract.Session;

namespace VMLab.Script.FluentInterface
{
    public interface ISession
    {
        IHost Host { get; }
        IEnumerable<IVMControl> VMs { get; }
        string LabName { get; }
        string LabAuthor { get; }
        string LabDescription { get; }
        
    }
}
