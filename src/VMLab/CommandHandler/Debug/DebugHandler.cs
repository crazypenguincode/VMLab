using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;

namespace VMLab.CommandHandler
{
    public class DebugHandler : IParamHandler
    {

        private IParamHandler[] _handlers;

        public string Group => "root";
        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            _handlers = handlers.ToArray();

            if (args.Length < 2)
                return false;

            return args[0].ToLower() == "debug";
        }

        public void Handle(string[] args)
        {
            var useableHandler = _handlers.FirstOrDefault(h => h.Group == "debug" && h.CanHandle(args.Skip(1).ToArray(), _handlers));

            if (useableHandler != null)
                useableHandler.Handle(args);
            else
                throw new NotImplementedException("Useage handler needs to be created here");
        }
    }
}
