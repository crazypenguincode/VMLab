using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.Helper;

namespace VMLab.CommandHandler.Template
{
    public class RemoveTemplateHandler : BaseParamHandler
    {
        private readonly IVMBuilder _builder;

        public RemoveTemplateHandler(IVMBuilder builder, IUsage usage) : base(usage)
        {
            _builder = builder;
        }

        public override string Group => "template";
        public override string[] Handles => new []{"remove", "r"};

        public override void OnHandle(string[] args)
        {
            _builder.RemoveTemplate(args[1]);
        }

        public override string UsageDescription => "Removes a template from vmlab.";
    }
}
