using System;
using System.Collections.Generic;
using VMLab.Script.FluentInterface;

namespace VMLab.GraphModels
{
    public class VM
    {
        public string Template { get; set; }
        public string Version { get; set; }
        public List<Credential> Credentials { get; set; }
        public List<Network> Networks { get; set; }
        public int CPUs { get; set; }
        public int CPUCores { get; set; }
        public int Memeory { get; set; }
        public DSC DSC { get; set; }
        public List<SharedFolder> SharedFolders { get; set; }
        public string Name { get; set; }
        public Action<IVMControl> OnProvision { get; set; }
        public Action<IVMControl> OnDestroy { get; set; }
        public System.Action AfterDestroy { get; set; }
        public IDictionary<string, string> Properties { get; }
        public bool NestedVirtualization { get; set; }

        public VM()
        {
            Credentials = new List<Credential>();
            Networks = new List<Network>();
            SharedFolders = new List<SharedFolder>();
            OnProvision = control => { };
            OnDestroy = control => { };
            AfterDestroy = () => { };
            Properties = new Dictionary<string, string>();
        }
    }
}
