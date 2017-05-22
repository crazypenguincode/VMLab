using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler
{
    public abstract class BaseParamHandler : IParamHandler
    {
        protected IEnumerable<IParamHandler> Handlers;
        public abstract string Group { get; }
        public abstract string[] Handles { get; }

        protected readonly IUsage Usage;

        protected BaseParamHandler(IUsage usage)
        {
            Usage = usage;
        }

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args == null || args.Length == 0)
                return false;

            Handlers = handlers;
            return Handles.Any(h => h == args[0].ToLower());
        }

        public void Handle(string[] args)
        {
            OnHandle(args);
        }


        public abstract void OnHandle(string[] args);
        public virtual Dictionary<string, string> UsageItems => new Dictionary<string, string>();
        public abstract string UsageDescription { get; }
        public virtual string UsageName => Handles.First();
    }
}
