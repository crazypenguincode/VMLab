using System.Collections.Generic;

namespace VMLab.CommandHandler
{
    public interface IParamHandler
    {
        string Group { get; }
        bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers);
        void Handle(string[] args);

        Dictionary<string, string> UsageItems { get; }
        string UsageDescription { get; }
        string UsageName { get; }
        
    }
}
