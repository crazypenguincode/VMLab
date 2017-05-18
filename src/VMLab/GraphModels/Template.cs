using System.Collections.Generic;

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
        Windows7
    }

    public enum Arch
    {
        X64,
        X86
    }

    public class Template
    {
        public Hypervisor Hypervisor { get; set; }
        public List<Credential> Credentials { get; set; }
        public ISO ISO { get; set; }
        public GuestOS GuestOS { get; set; }
        public Arch Arch { get; set; }
        public List<HardDisk> HardDisks { get; set; }
        public int Memory { get; set; }
        public int CPUs { get; set; }
        public int CPUCores { get; set; }
        public List<FloppyFile> FloppyFiles { get; set; }
    }
}
