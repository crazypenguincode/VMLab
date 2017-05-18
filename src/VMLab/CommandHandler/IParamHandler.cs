using System.Collections.Generic;

namespace VMLab.CommandHandler
{
    public interface IParamHandler
    {

        string Group { get; }
        bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers);
        void Handle(string[] args);
    }
}
