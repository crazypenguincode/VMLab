using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;
using VixCOM;
using VMLab.Contract.Helpers;

namespace VMLab.Hypervisor.VMwareWorkstation.VIX
{
    public class VIX : IVIX
    {
        private readonly VixLib _lib;
        private readonly IHost3 _host;
        private readonly ILogger _log;

        public VIX(ILogger log)
        {
            _log = log;
            _lib = new VixLibClass();
            _host = WaitJobResult<IHost3>(_lib.Connect(Constants.VIX_API_VERSION, Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION, null, 0, null,
                null, 0, null, null));
        }

        public IVM2 ConnectToVM(string path)
        {
            _log.Information("Connecting to VM {vm}", path);
            return WaitJobResult<IVM2>(_host.OpenVMEx(path, Constants.VIX_VMOPEN_NORMAL, null, null));
        }

        public void RemoveSnapshot(IVM2 vm, ISnapshot snapshot, bool removechildren)
        {
            _log.Information("Removing snapshot {snapshot}. Children: {children}", snapshot, removechildren);
            WaitJobNoResults(vm.RemoveSnapshot(snapshot, removechildren ? Constants.VIX_SNAPSHOT_REMOVE_CHILDREN : 0, null));
        }

        public void CreateSnapshot(IVM2 vm, string name, string description, bool capturememory)
        {
            _log.Information("Creating snapshot {snapshotname}", name);
            WaitJobNoResults(vm.CreateSnapshot(name, description, capturememory ? Constants.VIX_SNAPSHOT_INCLUDE_MEMORY : 0, null, null));
        }

        public void RevertToSnapshot(IVM2 vm, ISnapshot snapshot, bool supressPoweron)
        {
            _log.Information("Reverting to snapshot {snapshot}", snapshot);
            WaitJobNoResults(vm.RevertToSnapshot(snapshot, supressPoweron ? Constants.VIX_VMPOWEROP_SUPPRESS_SNAPSHOT_POWERON : 0, null, null));
        }

        public IEnumerable<ISnapshot> GetSnapshots(IVM2 vm)
        {
            _log.Information("Getting snapshots from vm:");
            var result = new List<ISnapshot>();
            CheckError(vm.GetNumRootSnapshots(out int snapshotscount));

            for (var i = 0; i < snapshotscount; i++)
            {
                CheckError(vm.GetRootSnapshot(i, out ISnapshot snapshot));
                result.Add(snapshot);
                result.AddRange(GetSubSnapshot(snapshot));
            }

            _log.Information("Snapshots: {@snapshot}", result);
            return result;
        }

        private IEnumerable<ISnapshot> GetSubSnapshot(ISnapshot snapshot)
        {
            _log.Information("Getting sub snapshots from vm");

            var results = new List<ISnapshot>();
            CheckError(snapshot.GetNumChildren(out int snapshotcount));

            for (var i = 0; i < snapshotcount; i++)
            {
                CheckError(snapshot.GetChild(i, out ISnapshot subsnapshot));
                results.Add(subsnapshot);
                results.AddRange(GetSubSnapshot(subsnapshot));
            }

            _log.Information("Snapshots: {@snapshot}", results);

            return results;
        }

        public IVM2 Clone(string path, IVM2 vm, ISnapshot snapshot, bool linked)
        {
            _log.Information("Cloning VM from snapshot {snapshot} to {vm}. Linked: {linked}", snapshot, path, linked);
            return
                WaitJobResult<IVM2>(vm.Clone(snapshot,
                    linked ? Constants.VIX_CLONETYPE_LINKED : Constants.VIX_CLONETYPE_FULL, path, 0, null, null));
        }

        public VixPowerState GetState(IVM2 vm)
        {
            var result = default(object);
            CheckError(((IVixHandle2)vm).GetProperties(new[] { Constants.VIX_PROPERTY_VM_POWER_STATE }, ref result));

            var state = (int)((object[])result)[0];

            if (CheckBitFlags(state, Constants.VIX_POWERSTATE_POWERED_ON | Constants.VIX_POWERSTATE_TOOLS_RUNNING))
                return VixPowerState.Ready;
            if (CheckBitFlags(state, Constants.VIX_POWERSTATE_POWERED_OFF))
                return VixPowerState.Off;
            return CheckBitFlags(state, Constants.VIX_POWERSTATE_SUSPENDED) ?
                VixPowerState.Suspended : VixPowerState.Pending;
        }

        private static bool CheckBitFlags(int value, int checkedvalue)
        {
            return (value | checkedvalue) == value;
        }

        private void CheckError(ulong err)
        {
            if (_lib.ErrorIndicatesFailure(err))
                throw new VixException($"Failed to run vix job. Error: {_lib.GetErrorText(err, null)} ({err})", err);
        }

        private T WaitJobResult<T>(IJob job)
        {
            return WaitJobResult<T>(job, new[] { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE });
        }

        private T WaitJobResult<T>(IJob job, int[] properites)
        {
            var results = default(object);
            var err = job.Wait(properites, ref results);

            CheckError(err);

            CloseObject(job);

            return (T)((object[])results)[0];
        }

        private void WaitJobNoResults(IJob job)
        {
            var err = job.WaitWithoutResults();

            CheckError(err);

            CloseObject(job);
        }

        public void CloseObject(object vixObject)
        {
            try
            {
                ((IVixHandle2)vixObject).Close();
            }
            catch (Exception)
            {
                //Close is not supported on some objects. Ignore any errors.
            }
        }

        public IEnumerable<string> GetAllRunning()
        {
            var callback = new RunningVMCallback(_lib);
            WaitJobNoResults(_host.FindItems(Constants.VIX_FIND_RUNNING_VMS, null, -1, callback));

            _log.Information("All running VMs: {@vms}", callback.RunningVMs);

            return callback.RunningVMs;
        }

        public void LoginToGuest(IVM2 vm, string username, string password, bool interactive)
        {
            _log.Information("Logging into VM Guest Username: {username} Password:Redacted Inteactive: {interactive}", username, interactive);
            WaitJobNoResults(vm.LoginInGuest(username, password, interactive ?
                Constants.VIX_LOGIN_IN_GUEST_REQUIRE_INTERACTIVE_ENVIRONMENT : 0, null));
        }

        public void LogoutOfGuest(IVM2 vm)
        {
            _log.Information("Logging out of guest");
            WaitJobNoResults(vm.LogoutFromGuest(null));
        }

        public bool FileExists(IVM2 vm, string path)
        {
            return WaitJobResult<bool>(vm.FileExistsInGuest(path, null), new[] { Constants.VIX_PROPERTY_JOB_RESULT_GUEST_OBJECT_EXISTS })
                .LogWithObject(o => _log.Information("Checking if file {file} exists in vm. Result: {result}", path, o));
        }

        public bool DirectoryExist(IVM2 vm, string path)
        {
            return WaitJobResult<bool>(vm.DirectoryExistsInGuest(path, null),
                    new[] {Constants.VIX_PROPERTY_JOB_RESULT_GUEST_OBJECT_EXISTS})
                .LogWithObject(o => _log.Information("Checking if directory {directory} exists in vm. Result: {result}", path, o));
        }

        public void CopyFileToGuest(IVM2 vm, string hostpath, string guestpath)
        {
            _log.Information("Copying host file {source} to guest {dest}.", hostpath, guestpath);
            WaitJobNoResults(vm.CopyFileFromHostToGuest(hostpath, guestpath, 0, null, null));
        }

        public void CopyFileToHost(IVM2 vm, string guestpath, string hostpath)
        {
            _log.Information("Copying guest file {source} to host {dest}.", guestpath, hostpath);
            WaitJobNoResults(vm.CopyFileFromGuestToHost(guestpath, hostpath, 0, null, null));
        }

        public void DeleteFileInGuest(IVM2 vm, string path)
        {
            _log.Information("Deleting file {file} in guest.", path);
            WaitJobNoResults(vm.DeleteFileInGuest(path, null));
        }

        public void RenameFileInGuest(IVM2 vm, string path, string newname)
        {
            _log.Information("Renaming file {file} in guest to {newname}.", path, newname);
            WaitJobNoResults(vm.RenameFileInGuest(path, newname, 0, null, null));
        }

        public void DeleteDirectoryInGuest(IVM2 vm, string path)
        {
            _log.Information("Deleting directory {directory} in guest.", path);
            WaitJobNoResults(vm.DeleteDirectoryInGuest(path, 0, null));
        }

        public void PowerOn(IVM2 vm)
        {
            _log.Information("Powering VM on.");
            WaitJobNoResults(vm.PowerOn(Constants.VIX_VMPOWEROP_NORMAL, null, null));
        }

        public void PowerOff(IVM2 vm, bool force)
        {
            _log.Information("Powering VM off.");
            WaitJobNoResults(vm.PowerOff(force ? Constants.VIX_VMPOWEROP_NORMAL : Constants.VIX_VMPOWEROP_FROM_GUEST, null));
        }

        public void Restart(IVM2 vm, bool force)
        {
            _log.Information("Restarting VM.");
            WaitJobNoResults(vm.Reset(force ? Constants.VIX_VMPOWEROP_NORMAL : Constants.VIX_VMPOWEROP_FROM_GUEST, null));
        }

        public void Delete(IVM2 vm)
        {
            _log.Information("Deleting VM.");
            WaitJobNoResults(vm.Delete(Constants.VIX_VMDELETE_DISK_FILES, null));
        }

        public long ExecuteCommand(IVM2 vm, string path, string args, bool activewindow, bool wait)
        {
            _log.Information("Executing command in VM Path: {path} Arguments: {args} ActiveWindow: {activewindow} Wait: {wait}.", path, args, activewindow, wait);
            var flags = 0;

            if (activewindow)
                flags = flags | Constants.VIX_RUNPROGRAM_ACTIVATE_WINDOW;
            if (!wait)
                flags = flags | Constants.VIX_RUNPROGRAM_RETURN_IMMEDIATELY;

            return WaitJobResult<long>(vm.RunProgramInGuest(path, args, flags, null, null), new[] { Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_ID });
        }

        public void EnableSharedFolders(IVM2 vm)
        {
            _log.Information("Enabling shared folders in VM");
            WaitJobNoResults(vm.EnableSharedFolders(true, 0, null));
        }

        public void DisableSharedFolders(IVM2 vm)
        {
            _log.Information("Disabling shared folders in VM");
            WaitJobNoResults(vm.EnableSharedFolders(false, 0, null));
        }

        public void AddSharedFolder(IVM2 vm, string path, string sharename, bool writeaccess)
        {
            _log.Information("Adding shared folder to VM. Sharename: {sharename} HostPath: {path} WriteAccess: {writeaccess}", sharename, path, writeaccess);
            WaitJobNoResults(vm.AddSharedFolder(sharename, path, writeaccess ? Constants.VIX_SHAREDFOLDER_WRITE_ACCESS : 0, null));
        }

        public void RemoveSharedFolder(IVM2 vm, string sharename)
        {
            _log.Information("Removing share {sharename}", sharename);
            WaitJobNoResults(vm.RemoveSharedFolder(sharename, 0, null));
        }

        public void WaitForTools(IVM2 vm)
        {
            _log.Information("Waiting for VMTools to become ready!");
            WaitJobNoResults(vm.WaitForToolsInGuest(int.MaxValue, null));
        }

        public string GetSnapshotName(ISnapshot snapshot)
        {
            var result = default(object);
            CheckError(((IVixHandle2)snapshot).GetProperties(new[] { Constants.VIX_PROPERTY_SNAPSHOT_DISPLAYNAME }, ref result));

            return ((object[])result)[0].ToString()
                .LogWithObject(o => _log.Information("Getting snapshot name... {snapshot}", o));
        }

        public string ReadVariable(IVM2 vm, string name, VixVariable environment)
        {
            var flags = 0;

            if (environment == VixVariable.Environment)
                flags = Constants.VIX_GUEST_ENVIRONMENT_VARIABLE;
            if (environment == VixVariable.GuestVar)
                flags = Constants.VIX_VM_GUEST_VARIABLE;
            if (environment == VixVariable.VMX)
                flags = Constants.VIX_VM_CONFIG_RUNTIME_ONLY;

            return WaitJobResult<string>(vm.ReadVariable(flags, name, 0, null), new[] { Constants.VIX_PROPERTY_JOB_RESULT_VM_VARIABLE_STRING })
                .LogWithObject(o => _log.Information("Getting Variable Name: {name} Environment: {environment} Value: {value}", name, environment, o));
        }

        public void WriteVariable(IVM2 vm, string name, string value, VixVariable environment)
        {
            var flags = 0;

            if (environment == VixVariable.Environment)
                flags = Constants.VIX_GUEST_ENVIRONMENT_VARIABLE;
            if (environment == VixVariable.GuestVar)
                flags = Constants.VIX_VM_GUEST_VARIABLE;
            if (environment == VixVariable.VMX)
                flags = Constants.VIX_VM_CONFIG_RUNTIME_ONLY;

            _log.Information("Writing variable Name: {name} Environment: {environment} Value: {value}", name, environment, value);
            WaitJobNoResults(vm.WriteVariable(flags, name, value, 0, null));
        }

        public void OpenLocalUI(string vmx, string vmwarepath)
        {
            var filename = $"{vmwarepath}\\vmware.exe";
            var args = $"-t -q \"{vmx}\"";

            _log.Information("Loading VMware UI with path: {filename} args: {args}", filename, args);

            Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args
            });
        }

        public IEnumerable<VixProcess> GetProcesses(IVM2 vm)
        {
            _log.Information("Getting processes in vm:");

            var returndata = new List<VixProcess>();
            var job = vm.ListProcessesInGuest(0, null);
            CheckError(job.WaitWithoutResults());

            var proccount = job.GetNumProperties(Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME);

            for (var i = 0; i < proccount; i++)
            {
                var result = default(object);
                CheckError(job.GetNthProperties(i, new[]
                {
                    Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_ID,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_OWNER,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_COMMAND
                }, ref result));

                var unpackedresult = (object[])result;

                var item = new VixProcess
                {
                    Name = unpackedresult[0] as string,
                    ProcessID = (long) unpackedresult[1],
                    Owner = unpackedresult[2] as string,
                    Command = unpackedresult[3] as string
                };

                returndata.Add(item);
                _log.Information("{@process}", item);
            }


            return returndata;
        }

        public void KillProcess(IVM2 vm, ulong processID)
        {
            _log.Information("Killing process with PID {pid}", processID);
            WaitJobNoResults(vm.KillProcessInGuest(processID, 0, null));
        }
    }

    public class RunningVMCallback : ICallback
    {
        private readonly IVixLib _lib;

        public List<string> RunningVMs { get; }

        public RunningVMCallback(IVixLib lib)
        {
            _lib = lib;
            RunningVMs = new List<string>();
        }

        public void OnVixEvent(IJob job, int eventType, IVixHandle moreEventInfo)
        {
            if (eventType != Constants.VIX_EVENTTYPE_FIND_ITEM)
                return;

            object results = null;

            var err = moreEventInfo.GetProperties(new[] { Constants.VIX_PROPERTY_FOUND_ITEM_LOCATION }, ref results);

            if (_lib.ErrorIndicatesFailure(err))
                return;

            var vmname = (string)((object[])results)[0];
            RunningVMs.Add(vmname);
        }

    }
}
