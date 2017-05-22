using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler.List
{
    public class ListHandler : HubParamHandler
    {
        public override string Group => "root";
        public override string UsageDescription => "This set of commands handles managment of hypervisors.";
        public override string[] Handles => new[] {"hypervisor", "h"};

        public ListHandler(IUsage usage) : base(usage)
        {
        }

        protected override string SubGroup => "hypervisor";
    }
}
