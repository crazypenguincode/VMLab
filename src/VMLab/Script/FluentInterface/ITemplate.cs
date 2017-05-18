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
        ITemplate HardDisk(string type, int size);
        ITemplate Network(string type, string name="");
        ITemplate CPU(int cpus, int cores);
        ITemplate Memory(int size);
        ITemplate FloppyFile(string sourcePath, string destinationName);
        ITemplate WithHypervisor(string hypervisor, Action<ITemplate> action);
        ITemplate OnProvision(Action<IVMControl> action);
    }
}
