using System.Collections.Generic;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract
{
    public interface ISessionFactory
    {
        ISession Build(IGraphManager graph);
    }
}
