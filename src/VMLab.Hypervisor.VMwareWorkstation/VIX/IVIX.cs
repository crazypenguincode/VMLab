using System.Collections.Generic;
using VixCOM;

namespace VMLab.Hypervisor.VMwareWorkstation.VIX
{
    public enum VixVariable
    {
        Environment,
        VMX,
        GuestVar
    }

    public enum VixPowerState
    {
        Off,
        Ready,
        Pending,
        Suspended
    }

    public interface IVIX
    {
        void AddSharedFolder(IVM2 vm, string path, string sharename, bool writeaccess);
        IVM2 Clone(string path, IVM2 vm, ISnapshot snapshot, bool linked);
        void CloseObject(object vixObject);
        IVM2 ConnectToVM(string path);
        void CopyFileToGuest(IVM2 vm, string hostpath, string guestpath);
        void CopyFileToHost(IVM2 vm, string guestpath, string hostpath);
        void CreateSnapshot(IVM2 vm, string name, string description, bool capturememory);
        void Delete(IVM2 vm);
        void DeleteDirectoryInGuest(IVM2 vm, string path);
        void DeleteFileInGuest(IVM2 vm, string path);
        void RenameFileInGuest(IVM2 vm, string path, string newname);
        bool DirectoryExist(IVM2 vm, string path);
        void DisableSharedFolders(IVM2 vm);
        void EnableSharedFolders(IVM2 vm);
        long ExecuteCommand(IVM2 vm, string path, string args, bool activewindow, bool wait);
        bool FileExists(IVM2 vm, string path);
        IEnumerable<string> GetAllRunning();
        IEnumerable<VixProcess> GetProcesses(IVM2 vm);
        IEnumerable<ISnapshot> GetSnapshots(IVM2 vm);
        VixPowerState GetState(IVM2 vm);
        void KillProcess(IVM2 vm, ulong processID);
        void LoginToGuest(IVM2 vm, string username, string password, bool interactive);
        void LogoutOfGuest(IVM2 vm);
        void PowerOff(IVM2 vm, bool force);
        void PowerOn(IVM2 vm);
        void RemoveSharedFolder(IVM2 vm, string sharename);
        void RemoveSnapshot(IVM2 vm, ISnapshot snapshot, bool removechildren);
        void Restart(IVM2 vm, bool force);
        void RevertToSnapshot(IVM2 vm, ISnapshot snapshot, bool supressPoweron);
        void WaitForTools(IVM2 vm);
        void WaitForTools(IVM2 vm, int timeout);
        string GetSnapshotName(ISnapshot snapshot);
        string ReadVariable(IVM2 vm, string name, VixVariable environment);
        void WriteVariable(IVM2 vm, string name, string value, VixVariable environment);
        void OpenLocalUI(string vmx, string vmwarepath);

    }
}
