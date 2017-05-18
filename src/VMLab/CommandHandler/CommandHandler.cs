using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IParamHandler[] _handlers;

        public CommandHandler(IEnumerable<IParamHandler> handlers)
        {
            _handlers = handlers.ToArray();
        }

        public void Parse(string[] args)
        {
            var useableHandler = _handlers.FirstOrDefault(h => h.Group == "root" && h.CanHandle(args, _handlers));

            if(useableHandler != null)
                useableHandler.Handle(args);
            else
                throw new NotImplementedException("Useage handler needs to be created here");
        }
    }
}
