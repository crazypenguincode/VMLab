using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler
{
    public class DebugHandler : HubParamHandler
    {
        public override string Group => "root";
        public override string UsageDescription => "Debug commands used for troubleshooting vmlab";
        protected override string SubGroup => "debug";
        public override string[] Handles => new[] { "debug", "d" };

        public DebugHandler(IUsage usage) : base(usage)
        {
        }

        
    }
}
