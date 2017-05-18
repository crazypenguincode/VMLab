using System;
using System.Collections.Generic;

namespace VMLab.CommandHandler
{
    public class BuildHandler : IParamHandler
    {
        public string Group => "root";

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 1)
                return false;

            return args[0].ToLower() == "build";
        }

        public void Handle(string[] args)
        {
            //build graph

            //call hypervisor vm builder.

            //start vm

            //wait for vm to be ready for provisioning

            //run provision script

            //run vm cleaner

            //archive template.
        }
    }
}
