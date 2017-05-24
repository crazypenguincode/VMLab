using System;
using VMLab.GraphModels;

namespace VMLab.Script.FluentInterface
{
    public interface ITemplate
    {
        string Name { get; set; }
        string Version { get; set; }

        ITemplate Credential(string group, string username, string password);
        ITemplate ISO(string path, string url);
        ITemplate GuestOS(string os, string arch);
        ITemplate HardDisk(int size);
        ITemplate Network(string type, string name="");
        ITemplate CPU(int cpus, int cores);
        ITemplate Memory(int size);
        ITemplate FloppyImage(string path);
        ITemplate WithHypervisor(string hypervisor, Action<ITemplate> action);
        ITemplate OnProvision(Action<IVMControl> action);
        ITemplate Healdess(bool headless);
    }
}
