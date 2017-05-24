using System.Linq;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract.Session
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IVMManager _vmManager;
        private readonly IHost _host;

        public SessionFactory(IVMManager vmManager, IHost host)
        {
            _vmManager = vmManager;
            _host = host;
        }

        public ISession Build(IGraphManager graph) => 
             new Session(graph.VMs.Select(v => _vmManager.GetVM(v)).Where(v => v != null), graph.LabName,
                graph.LabAuthor, graph.LabDescription, _host);

    }
}
