using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SystemInterface.IO;

namespace VMLab.Helper
{
    public class HypervisorFinder : IHypervisorFinder
    {
        private readonly IDirectory _directory;

        public HypervisorFinder(IDirectory directory)
        {
            _directory = directory;
        }

        public string[] Hypervisors
        {
            get
            {
                var asmFolder = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

                return (from file in _directory.GetFiles(asmFolder).Where(f => f != null && Path.GetFileName(f).ToLower().StartsWith("vmlab.hypervisor."))
                        where file != null
                        where file.ToLower().EndsWith(".dll")
                        select Path.GetFileName(file).ToLower().Replace("vmlab.hypervisor.", "").Replace(".dll", "") 
                        into hypervisor
                        select hypervisor.Substring(0, 1).ToUpper() + hypervisor.Substring(1)).ToArray();
            }
        }
    }
}
