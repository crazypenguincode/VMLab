using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract.GraphModels;
using VMLab.Contract.OSEnvironment;
using VMLab.Contract.Shim;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Hypervisor.VMwareWorkstation.VIX;
using VMLab.Hypervisor.VMwareWorkstation.VMX;
using VMLab.Script.FluentInterface;
using IConsole = VMLab.Helper.IConsole;

namespace VMLab.Hypervisor.VMwareWorkstation.VM
{
    public class VMControl : IVMControl
    {
        private readonly IVIX _vix;

        private string _vmx;
        private List<Credential> _credentials;
        private Credential _currentCredential;
        private readonly Func<IExecutionShim> _shimFactory;
        private readonly IConfig _config;
        private readonly IFile _file;
        private readonly IConsole _console;
        private readonly IEnvironment _environment;
        private TemplateManifest _manifest;
        private GraphModels.VM _vm;
        private readonly IOSEnvironmentManager _osEnvironmentManager;

        public VMControl(IVIX vix, Func<IExecutionShim> shimFactory, IConfig config, IFile file, IConsole console, IEnvironment environment, IOSEnvironmentManager osEnvironmentManager)
        {
            _vix = vix;
            _shimFactory = shimFactory;
            _config = config;
            _file = file;
            _console = console;
            _environment = environment;
            _osEnvironmentManager = osEnvironmentManager;
        }

        internal void SetCredentials(IEnumerable<Credential> credentials)
        {
            _credentials = new List<Credential>(credentials);
        }

        internal void SetVMXFile(string vmx, TemplateManifest manifest, GraphModels.VM vm)
        {
            _vmx = vmx;
            _manifest = manifest;
            _vm = vm;
        }

        public void Exec(string path, string args, bool wait = true)
        {
            Exec(path, args, null, wait);
        }

        public void Exec(string path, string args, Action<IVMControl, IExecResult> execResult = null, bool wait = true)
        {
            var retry = true;
            var osEnv = _osEnvironmentManager.GetOSEnvironment(OS, Arch);
            while (retry)
            {
                retry = false;
                var vm = _vix.ConnectToVM(_vmx);

                if (!wait)
                {
                    if (execResult != null)
                    {
                        throw new ApplicationException("Can't use execResult with wait set to null");
                    }

                    _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
                    _vix.ExecuteCommand(vm, path, args, false, false);
                    _vix.LogoutOfGuest(vm);
                }
                else
                {
                    var shim = _shimFactory();
                    shim.OSEnvironment = osEnv;
                    shim.ExecutionAction = (p, a) => _vix.ExecuteCommand(vm, p, a, false, false);
                    shim.FileExist = p => _vix.FileExists(vm, p);
                    shim.GetFileAction = file =>
                    {

                        var localfile = $"{_config.GetSetting("TempDir")}\\{Path.GetFileName(file)}";

                        _vix.CopyFileToHost(vm, file, localfile);

                        return _file.ReadAllLines(localfile);
                    };
                    shim.PutFileAction = (local, guest) => _vix.CopyFileToGuest(vm, local, guest);
                    shim.RemoveFile = p => _vix.DeleteFileInGuest(vm, p);

                    _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);

                    var returnCode = shim.Execute(path, args);
                    var retObj = new ExecResult {ReturnCode = returnCode};

                    if (execResult != null)
                    {

                        execResult(this, retObj);
                        retry = retObj.Retry;

                        if (!string.IsNullOrEmpty(retObj.FailMessage))
                        {
                            _console.Error(retObj.FailMessage);
                            throw new ApplicationException("Failed to execute action! See log for details.");
                        }

                        _console.Information(retObj.SuccessMessage);
                    }

                    _vix.LogoutOfGuest(vm);
                }

                _vix.CloseObject(vm);
            }
        }

        public void Powershell(string path, bool wait = true)
        {
            Powershell(path, null, wait);
        }

        public void Powershell(string path, Action<IVMControl, IExecResult> execResult, bool wait = true)
        {
            var osEnv = _osEnvironmentManager.GetOSEnvironment(OS, Arch);
            var id = Guid.NewGuid().ToString();
            var guestfile = $"{osEnv.TempDirectory}{id}.ps1";
            var localFile = $"{_environment.CurrentDirectory}\\{path}";
            
            CopyFileToVM(localFile, guestfile);

            Exec($"{osEnv.PowershellExe}", $"{osEnv.PowershellArgs} \"{guestfile}\"", execResult, wait);
        }

        public void Wait(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public void WaitReady()
        {
            var vm = _vix.ConnectToVM(_vmx);

            while (true)
            {
                try
                {
                    _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
                    if (_vix.GetProcesses(vm).ToArray().Length != 0)
                        break;

                    _vix.LogoutOfGuest(vm);
                }
                catch
                {
                    //do nothing while we wait.
                }

                Thread.Sleep(1000);
            }
            _vix.LogoutOfGuest(vm);
            _vix.CloseObject(vm);
                
        }

        public void WaitPowerOff()
        {
            var vm = _vix.ConnectToVM(_vmx);

            while (_vix.GetState(vm) != VixPowerState.Off)
            {
                Thread.Sleep(1000);
            }

            _vix.CloseObject(vm);
        }

        public void WaitFile(string path, bool exists = true)
        {
            var vm = _vix.ConnectToVM(_vmx);

            while (true)
            {
                try
                {
                    _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
                    if (_vix.FileExists(vm, path) == exists)
                    {
                        break;
                    }
                    _vix.LogoutOfGuest(vm);
                }
                catch
                {
                    //skipping errors.
                }

                Thread.Sleep(1000);
            }

            _vix.LogoutOfGuest(vm);
            _vix.CloseObject(vm);

        }

        public void Restart(bool force = false)
        {
            var vm = _vix.ConnectToVM(_vmx);

            if (PowerState == VMPower.Off)
                return;

            if (!force)
                WaitReady();

            _vix.Restart(vm, force);
            _vix.CloseObject(vm);
        }

        public void Stop(bool force = false)
        {
            var vm = _vix.ConnectToVM(_vmx);

            if (PowerState == VMPower.Off)
                return;

            if(!force)
                WaitReady();

            _vix.PowerOff(vm, force);
            _vix.CloseObject(vm);
        }

        public void Start()
        {
            var vm = _vix.ConnectToVM(_vmx);
            _vix.PowerOn(vm);
            _vix.CloseObject(vm);
        }

        public void CopyFileToVM(string hostPath, string guestPath)
        {
            var vm = _vix.ConnectToVM(_vmx);
            _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
            _vix.CopyFileToGuest(vm, hostPath, guestPath);
            _vix.LogoutOfGuest(vm);
            _vix.CloseObject(vm);
        }

        public void CopyFileFromVM(string guestPath, string hostPath)
        {
            var vm = _vix.ConnectToVM(_vmx);
            _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
            _vix.CopyFileToHost(vm, guestPath, hostPath);
            _vix.LogoutOfGuest(vm);
            _vix.CloseObject(vm);
        }

        public void DeleteFileFromVM(string path)
        {
            var vm = _vix.ConnectToVM(_vmx);
            _vix.LoginToGuest(vm, _currentCredential.Username, _currentCredential.Password, false);
            _vix.DeleteFileInGuest(vm, path);
            _vix.LogoutOfGuest(vm);
            _vix.CloseObject(vm);
        }

        public IEnumerable<string> GetSnapshots()
        {
            var vm = _vix.ConnectToVM(_vmx);

            var snapshots = _vix.GetSnapshots(vm);

            var result = snapshots.Select(s => _vix.GetSnapshotName(s)).ToList();

            _vix.CloseObject(vm);

            return result;
        }

        public void NewSnapshot(string name)
        {
            var vm = _vix.ConnectToVM(_vmx);
            _vix.CreateSnapshot(vm, name, "Captured by vmlab", _vix.GetState(vm) != VixPowerState.Off);
            _vix.CloseObject(vm);
        }

        public void RemoveSnapshot(string name)
        {
            var vm = _vix.ConnectToVM(_vmx);
            var snapshot = _vix.GetSnapshots(vm).FirstOrDefault(s => _vix.GetSnapshotName(s) == name);

            if(snapshot != null)
                _vix.RemoveSnapshot(vm, snapshot, true);

            _vix.CloseObject(vm);
        }

        public void RevertToSnapshot(string name)
        {
            var vm = _vix.ConnectToVM(_vmx);
            var snapshot = _vix.GetSnapshots(vm).FirstOrDefault(s => _vix.GetSnapshotName(s) == name);

            if (snapshot != null)
                _vix.RevertToSnapshot(vm, snapshot, true);

            _vix.CloseObject(vm);
        }

        public void SetCredentials(string group)
        {
            var cred = _credentials.FirstOrDefault(c => string.Equals(c.Group, group, StringComparison.CurrentCultureIgnoreCase));

            if (cred != null)
                _currentCredential = cred;
        }

        public VMPower PowerState
        {
            get
            {
                var vm = _vix.ConnectToVM(_vmx);
                try { _vix.WaitForTools(vm, 1);} catch { /* skip errors as it will be because the vm is off. */}
                var state = _vix.GetState(vm);
                _vix.CloseObject(vm);

                switch (state)
                {
                    case VixPowerState.Off:
                        return VMPower.Off;
                    case VixPowerState.Ready:
                        return VMPower.Ready;
                    case VixPowerState.Pending:
                        return VMPower.Pending;
                    case VixPowerState.Suspended:
                        return VMPower.Off;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void AddSharedFolder(string name, string hostPath, string guestPath)
        {
            var osEnv = _osEnvironmentManager.GetOSEnvironment(OS, Arch);
            hostPath = Path.GetFullPath(hostPath);
            WaitReady();
            var vm = _vix.ConnectToVM(_vmx);
            _vix.EnableSharedFolders(vm);
            _vix.AddSharedFolder(vm, hostPath, name, true);

            Exec(osEnv.Shell, $"{osEnv.ShellPreArg}{osEnv.LinkCommand.Replace("$$GuestPath$$", guestPath).Replace("$$Name$$", name)}", true);
        }

        public void RemoveSharedFolder(string name, string hostPath, string guestPath)
        {
            var osEnv = _osEnvironmentManager.GetOSEnvironment(OS, Arch);
            WaitReady();
            var vm = _vix.ConnectToVM(_vmx);
            _vix.RemoveSharedFolder(vm, name);

            Exec(osEnv.Shell, $"{osEnv.ShellPreArg}{osEnv.RemoveDirectory.Replace("$$Folder$$", guestPath)}", true);
        }

        public GuestOS OS => _manifest.OS;
        public Arch Arch => _manifest.Arch;
        public string Name => _vm?.Name;
        public int Memory => _vm?.Memeory ?? -1;
        public int Cpu => _vm?.CPUs ?? -1;
        public int CpuCore => _vm?.CPUCores ?? -1;

        public void ShowUI()
        {
            _vix.OpenLocalUI(_vmx, "C:\\Program Files (x86)\\VMware\\VMware Workstation");
        }
    }
}
