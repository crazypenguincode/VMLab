using System;
using System.Collections.Generic;

namespace VMLab.Script.FluentInterface
{
    public interface IVMControl
    {
        void Exec(string path, string args, bool wait = true);
        void Exec(string path, string args, Action<IVMControl, IExecResult> execResult, bool wait = true);
        void Powershell(string path, bool wait = true);
        void Powershell(string path, Action<IVMControl, IExecResult> execResult, bool wait = true);
        void Wait(int seconds);
        void WaitReady();
        void WaitPowerOff();
        void WaitFile(string path, bool exists = true);
        void Restart(bool force = false);
        void Stop(bool force = false);
        void Start();
        void CopyFileToVM(string hostPath, string guestPath);
        void CopyFileFromVM(string guestPath, string hostPath);
        void DeleteFileFromVM(string path);
        IEnumerable<string> GetSnapshots();
        void NewSnapshot(string name);
        void RemoveSnapshot(string name);
        void RevertToSnapshot(string name);
        void ShowUI();
        void SetCredentials(string group);
    }
}
