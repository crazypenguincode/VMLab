using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler.Import
{
    public class TemplateHandler : HubParamHandler
    {
        public override string Group => "root";
        public override string UsageDescription => "Allows for installation and removal of templates.";
        public override string[] Handles => new[] {"template"};

        public TemplateHandler(IUsage usage) : base(usage)
        {
        }

        protected override string SubGroup => "template";
    }
}
