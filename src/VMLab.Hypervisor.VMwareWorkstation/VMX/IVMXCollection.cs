namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public interface IVMXCollection
    {
        void ReadFromFile(string path);
        string ReadValue(string name);
        void WriteToFile(string path);
        void WriteValue(string name, string value);
        void ClearValue(string pattern);
    }
}