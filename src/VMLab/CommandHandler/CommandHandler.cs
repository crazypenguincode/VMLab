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
            _handlers = handlers.Where(h => h.RootCommand).ToArray();
        }

        public void Parse(string[] args)
        {
            var useableHandler = _handlers.FirstOrDefault(h => h.CanHandle(args));

            if(useableHandler != null)
                useableHandler.Handle(args);
            else
                throw new NotImplementedException("Useage handler needs to be created here");
        }
    }
}
