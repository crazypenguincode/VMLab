using System;
using System.Collections.Generic;
using VMLab.Script.FluentInterface;

namespace VMLab.GraphModels
{
    public enum Hypervisor
    {
        VMwareWorkstation
    }

    public enum GuestOS
    {
        Nano,
        Windows2016,
        Windows2016Core,
        Windows2012R2,
        Windows2012R2Core,
        Windows2012,
        Windows2012Core,
        Windows2008R2,
        Windows2008R2Core,
        Windows10,
        Windows81,
        Windows8,
        Windows7,
        Ubuntu
    }

    public enum Arch
    {
        X64,
        X86
    }

    public class Template
    {
        public Hypervisor Hypervisor { get; set; }
        public ISO ISO { get; set; }
        public GuestOS GuestOS { get; set; }
        public Arch Arch { get; set; }
        public List<HardDisk> HardDisks { get; set; }
        public int Memory { get; set; }
        public int CPUs { get; set; }
        public int CPUCores { get; set; }
        public string FloppyImage { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<Network> Networks { get; set; }
        public Action<IVMControl> OnProvision { get; set; }
        public bool HeadLess { get; set; }
        public Template()
        {
            HardDisks = new List<HardDisk>();
            Networks = new List<Network>();
            OnProvision = control => { };
        }
    }
}
