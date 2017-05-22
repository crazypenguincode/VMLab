using System.Linq;

namespace VMLab.CommandHandler
{
    public abstract class HubParamHandler : BaseParamHandler
    {
        protected abstract string SubGroup { get; }

        public override void OnHandle(string[] args)
        {
            var useableHandler = Handlers.FirstOrDefault(h => h.Group == SubGroup && h.CanHandle(args.Skip(1).ToArray(), Handlers));

            if (useableHandler != null)
                useableHandler.Handle(args.Skip(1).ToArray());
            else
                Usage.WriteHubUsage(this, Handlers.ToArray());
        }

        protected HubParamHandler(IUsage usage) : base(usage)
        {

        }
    }
}
