using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class RemoveTemplateHandler : IParamHandler
    {
        private readonly IVMBuilder _builder;

        public RemoveTemplateHandler(IVMBuilder builder)
        {
            _builder = builder;
        }

        public string Group => "template";
        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 2)
                return false;

            return args[0].ToLower() == "remove";
        }

        public void Handle(string[] args)
        {
            _builder.RemoveTemplate(args[1]);

        }
    }
}
