using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler.Import
{
    public class ImportHandler : IParamHandler
    {
        private IParamHandler[] _handlers;

        public string Group => "root";
        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            _handlers = handlers.ToArray();

            if (args.Length < 2)
                return false;

            return args[0].ToLower() == "template";
        }

        public void Handle(string[] args)
        {
            var useableHandler = _handlers.FirstOrDefault(h => h.Group == "template" && h.CanHandle(args.Skip(1).ToArray(), _handlers));

            if (useableHandler != null)
                useableHandler.Handle(args.Skip(1).ToArray());
            else
                throw new NotImplementedException("Useage handler needs to be created here");
        }
    }
}
