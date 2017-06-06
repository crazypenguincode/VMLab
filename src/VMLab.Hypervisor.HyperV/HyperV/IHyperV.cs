using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV.HyperV
{
    public interface IHyperV
    {
        void NewVM(string name, string path, int generation = 2);
        void SetCPUCount(string vmname, int count);
        void SetMemory(string vmName, int memory, bool dynamic, int upper, int lower);
        void AddDisk(string vmName, string path, int size, int location);
        void AddDisk(string vmName, string path, int location);
        void SetFloppyImage(string vmName, string path);
        void AddNetwork(string vmName, string vswitch);
        void CreateDifferenceDisk(string source, string destination);
        void ExecPowerShell(string vmname, string path);
        void ExecuteCommand(string vmname, string path, string args, bool wait);
        bool FileExists(string vmname, string path);
        void WaitReady(string vmname);
        void Restart(string vmname, bool force);
        void Stop(string vmname, bool force);
        void Start(string vmname);
        VMPower PowerState(string vmname);
        string[] GetSnapshot(string vmname);
        void CreateSnapshot(string vmname, string name);
        void RevertSnapshot(string vmname, string name);
        void RemoveSnapshot(string vmname, string name);
    }
}
