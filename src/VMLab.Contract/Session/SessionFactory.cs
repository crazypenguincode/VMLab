using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract.Session
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IVMBuilder _vmBuilder;

        public SessionFactory(IVMBuilder vmBuilder)
        {
            _vmBuilder = vmBuilder;
        }

        public ISession Build(IGraphManager graph) => 
             new Session(graph.VMs.Select(v => _vmBuilder.GetVM(v)).Where(v => v != null), graph.LabName,
                graph.LabAuthor, graph.LabDescription);

    }
}
