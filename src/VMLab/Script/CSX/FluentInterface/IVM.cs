using System;

namespace VMLab.Script.FluentInterface
{
    public interface IVM
    {
        string Name { get; set; }
        IVM Template(string name, string version = "latest");
        IVM Credential(string group, string username, string password);
        IVM Network(string type, string name = "");
        IVM CPU(int cpus, int cores);
        IVM Memory(int size);
        IVM ShareFolder(string hostpath, string guestpath);
        IVM WithHypervisor(string hypervisor, Action<IVM> action);
        IVM OnProvision(Action<IVMControl> action);
        IVM OnDestroy(Action<IVMControl> action);
        IVM AfterDestroy(Action action);
    }
}
