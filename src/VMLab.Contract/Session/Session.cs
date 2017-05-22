using System.Collections.Generic;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract.Session
{
    public class Session : ISession
    {
        internal Session(IEnumerable<IVMControl> vMs, string labName, string labAuthor, string labDescription)
        {
            VMs = vMs;
            LabName = labName;
            LabAuthor = labAuthor;
            LabDescription = labDescription;
        }

        public IEnumerable<IVMControl> VMs { get; }
        public string LabName { get; }
        public string LabAuthor { get; }
        public string LabDescription { get; }
    }
}
