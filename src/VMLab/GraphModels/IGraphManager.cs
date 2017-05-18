using System.Collections.Generic;

namespace VMLab.GraphModels
{
    public interface IGraphManager
    {
        IEnumerable<Template> Templates { get; }
        IEnumerable<VM> VMs { get; }
        string LabName { get; set; }
        string LabAuthor { get; set; }
        string LabDescription { get; set; }
        IEnumerable<string> Locks { get; }

        void AddTemplate(Template template);
        void AddVM(VM vm);
        void AddLock(string name);
    }
}
