using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.Threading;
using VMLab.Contract.GraphModels;
using VMLab.GraphModels;
using VMLab.Hypervisor.HyperV.HyperV;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV
{
    public class VMControl : IVMControl
    {
        private readonly IHyperV _hyperv;

        private string _vmName;
        private TemplateManifest _manifest;
        private VM _vm;
        private IEnumerable<Credential> _credentials;
        private IThread _thread;
        private Credential _currentCredential;

        public VMControl(IHyperV hyperv, IThread thread)
        {
            _hyperv = hyperv;
            _thread = thread;
        }

        internal void SetVMData(string name, TemplateManifest manifest, VM vm, IEnumerable<Credential> creds)
        {
            _vmName = name;
            _manifest = manifest;
            _vm = vm;
            _credentials = creds;
        }
        public void Exec(string path, string args, bool wait = true)
        {
            _hyperv.ExecuteCommand(_vmName, path, args, wait);
        }

        public void Exec(string path, string args, Action<IVMControl, IExecResult> execResult, bool wait = true)
        {
            _hyperv.ExecuteCommand(_vmName, path, args, wait);
        }

        public void Powershell(string path, bool wait = true)
        {
            _hyperv.ExecPowerShell(_vmName, path);
        }

        public void Powershell(string path, Action<IVMControl, IExecResult> execResult, bool wait = true)
        {
            _hyperv.ExecPowerShell(_vmName, path);
        }

        public void Wait(int seconds)
        {
            _thread.Sleep(seconds * 1000);
        }

        public void WaitReady()
        {
            _hyperv.WaitReady(_vmName);
        }

        public void WaitPowerOff()
        {
            while (PowerState != VMPower.Off)
            {
                _thread.Sleep(3000);
            }
        }

        public void WaitFile(string path, bool exists = true)
        {
            while (true)
            {
                try
                {
                    if (_hyperv.FileExists(_vmName, path) == exists)
                        break;
                }
                catch
                {
                    //do nothing.
                }
            }
        }

        public void Restart(bool force = false)
        {
            _hyperv.Restart(_vmName, force);
        }

        public void Stop(bool force = false)
        {
            _hyperv.Stop(_vmName, force);
        }

        public void Start(bool runStartupHandlers = true)
        {
            _hyperv.Start(_vmName);
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
            return _hyperv.GetSnapshot(_vmName);
        }

        public void NewSnapshot(string name)
        {
            _hyperv.CreateSnapshot(_vmName, name);
        }

        public void RemoveSnapshot(string name)
        {
            _hyperv.RemoveSnapshot(_vmName, name);
        }

        public void RevertToSnapshot(string name)
        {
            _hyperv.RevertSnapshot(_vmName, name);
        }

        public void ShowUI()
        {
            _hyperv.ConnectUI(_vmName);
        }

        public void SetCredentials(string group)
        {
            var cred = _credentials.FirstOrDefault(c => string.Equals(c.Group, group, StringComparison.CurrentCultureIgnoreCase));

            if (cred != null)
                _currentCredential = cred;
        }

        public VMPower PowerState => _hyperv.PowerState(_vmName);

        public void AddSharedFolder(string name, string hostPath, string guestPath)
        {
            throw new NotImplementedException();
        }

        public void RemoveSharedFolder(string name, string hostPath, string guestPath)
        {
            throw new NotImplementedException();
        }

        public GuestOS OS => _manifest.OS;
        public Arch Arch => _manifest.Arch;
        public string Name => _vm?.Name;
        public int Memory => _vm?.Memeory ?? -1;
        public int Cpu => _vm?.CPUs ?? -1;
        public int CpuCore => _vm?.CPUCores ?? -1;

        public bool FileExistsInGuest(string path)
        {
            return _hyperv.FileExists(_vmName, path);
        }
    }
}

