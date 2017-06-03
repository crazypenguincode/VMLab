using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV
{
    public class VMControl : IVMControl
    {
        public void Exec(string path, string args, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public void Exec(string path, string args, Action<IVMControl, IExecResult> execResult, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public void Powershell(string path, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public void Powershell(string path, Action<IVMControl, IExecResult> execResult, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public void Wait(int seconds)
        {
            throw new NotImplementedException();
        }

        public void WaitReady()
        {
            throw new NotImplementedException();
        }

        public void WaitPowerOff()
        {
            throw new NotImplementedException();
        }

        public void WaitFile(string path, bool exists = true)
        {
            throw new NotImplementedException();
        }

        public void Restart(bool force = false)
        {
            throw new NotImplementedException();
        }

        public void Stop(bool force = false)
        {
            throw new NotImplementedException();
        }

        public void Start(bool runStartupHandlers = true)
        {
            throw new NotImplementedException();
        }

        public void CopyFileToVM(string hostPath, string guestPath)
        {
            throw new NotImplementedException();
        }

        public void CopyFileFromVM(string guestPath, string hostPath)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileFromVM(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSnapshots()
        {
            throw new NotImplementedException();
        }

        public void NewSnapshot(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveSnapshot(string name)
        {
            throw new NotImplementedException();
        }

        public void RevertToSnapshot(string name)
        {
            throw new NotImplementedException();
        }

        public void ShowUI()
        {
            throw new NotImplementedException();
        }

        public void SetCredentials(string @group)
        {
            throw new NotImplementedException();
        }

        public VMPower PowerState { get; }
        public void AddSharedFolder(string name, string hostPath, string guestPath)
        {
            throw new NotImplementedException();
        }

        public void RemoveSharedFolder(string name, string hostPath, string guestPath)
        {
            throw new NotImplementedException();
        }

        public GuestOS OS { get; }
        public Arch Arch { get; }
        public string Name { get; }
        public int Memory { get; }
        public int Cpu { get; }
        public int CpuCore { get; }
        public bool FileExistsInGuest(string path)
        {
            throw new NotImplementedException();
        }
    }
}
