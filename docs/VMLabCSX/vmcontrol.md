# VM Control Interface
The IVMControl interface is used to control virtual machines in OnProvision, OnDestroy, AfterDestroy or during custom actions in the vmlab.csx file.

## Methods.
Here are all the methods supported by the IVMControl template.

```

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
        void Start(bool runStartupHandlers = true);
        void CopyFileToVM(string hostPath, string guestPath);
        void CopyFileFromVM(string guestPath, string hostPath);
        void DeleteFileFromVM(string path);
        IEnumerable<string> GetSnapshots();
        void NewSnapshot(string name);
        void RemoveSnapshot(string name);
        void RevertToSnapshot(string name);
        void ShowUI();
        void SetCredentials(string group);
        VMPower PowerState { get; }
        void AddSharedFolder(string name, string hostPath, string guestPath);
        void RemoveSharedFolder(string name, string hostPath, string guestPath);
        GuestOS OS { get; }
        Arch Arch { get; }
        string Name { get; }
        int Memory { get; }
        int Cpu { get; }
        int CpuCore { get; }
        bool FileExistsInGuest(string path);
    }
```

---

### Exec
```
void Exec(string path, string args, bool wait = true);
void Exec(string path, string args, Action<IVMControl, IExecResult> execResult, bool wait = true);
```

Executes a command in VM.

##### Arguments:
* path - Path to executable to run.
* args - Arguments to pass to executable.
* wait - Determines if vmlab should wait for executable to terminate before continuing.
* execREsult - Action that is executed after command is executed to determine if its successful or not.

##### IExecResult
```
    public interface IExecResult
    {
        void Fail(string message = "Bad return code!");
        void Success(string message = "Command ran successfully!");
        bool Retry { get; set; }
        int ReturnCode { get; }
    }
```
* Fail - Call this to cause the command to fail with optional error message.
* Success - Call this to flag the command as successful with optional message.
* Retry - Set this to true to run the command again (useful for actions that require multiple attempts, like patching).
* Return Code - Is assigned to the return code of the process for checking.

##### Example
```
vm.Exec("c:\\myupdater.exe", "-silent", (v, ret) => {
    if(ret.ReturnCode == 3010 || ret.ReturnCode == -1)
    {
        vm.Restart();
        ret.Retry = true;
        Echo("Restarting and checking for more patches!");
    }
});

vm.Exec("c:\\myupdater.exe", "-silent");
```
---

### Powershell
```
void Powershell(string path, bool wait = true);
void Powershell(string path, Action<IVMControl, IExecResult> execResult, bool wait = true);
```

Executes a Powershell script in vm.

##### Arguments:
* path - Path to script (on host) to run.
* wait - Determines if vmlab should wait for powershell script to finish before continuing.
* execREsult - Action that is executed after command is executed to determine if its successful or not.

##### IExecResult
```
    public interface IExecResult
    {
        void Fail(string message = "Bad return code!");
        void Success(string message = "Command ran successfully!");
        bool Retry { get; set; }
        int ReturnCode { get; }
    }
```
* Fail - Call this to cause the command to fail with optional error message.
* Success - Call this to flag the command as successful with optional message.
* Retry - Set this to true to run the command again (useful for actions that require multiple attempts, like patching).
* Return Code - Is assigned to the return code of the process for checking.

##### Example
```
vm.Powershell("03-InstallUpdates.ps1", (v, ret) => {
    if(ret.ReturnCode == 3010 || ret.ReturnCode == -1)
    {
        vm.Restart();
        ret.Retry = true;
        Echo("Restarting and checking for more patches!");
    }
});

vm.Powershell("03-InstallUpdates.ps1");
```

---

### Wait
```
void Wait(int seconds);
```

Waits number of seconds before continuing.

##### Arguments:
* seconds - How many seconds to wait before continuing.

---

### WaitReady
```
void WaitReady();
```

Waits till VM is ready and Hypervisor can execute commands in it.

##### Arguments:
None

---

### WaitPowerOff
```
void WaitPowerOff();
```

Waits till VM is powered off before continuing.

##### Arguments:
None

---

### WaitFile
```
void WaitFile(string path, bool exists = true);
```

Waits till file exists or doesn't exist in vm. This is useful for a complex process that restarts the virtual machine multiple times or that you can't wait for an executable to finish.

##### Arguments:
* path - File in guest to check.
* exists - If true wait for file to exist before continuing otherwise wait for file to no exist.

### Restart
```
void Restart(bool force = false);
```

Restarts the virtual machine.

##### Arguments:
* force - If set to true it will force a restart without waiting for the operating system to do it.

---

### Stop
```
void Stop(bool force = false);
```

##### Arguments:
* force - If set to true it powers the vm off without giving the os a chance to shutdown.

---

### Start
```
void Start();
```

##### Arguments:
None

---

### CopyFileToVM
```
void CopyFileToVM(string hostPath, string guestPath);
```

Copies a file into the guest from the host.

##### Arguments:
* hostPath - Path on host of file to copy.
* guestPath - Path in guest to copy file to.

---

### CopyFileFromVM
```
void CopyFileFromVM(string guestPath, string hostPath);
```

Copies a file from the guest to the host.

##### Arguments:
* guestPath - Path in guest to copy file from.
* hostPath - Path on host to copy the file to.

---

### DeleteFileFromVM
```
void DeleteFileFromVM(string path);
```

Deletes a file from vm.

##### Arguments:
* Path - Path to file to remove.

---

### GetSnapshots
```
IEnumerable<string> GetSnapshots();
```

Retrieves all snapshots Virtual Machine has.

##### Arguments:
None

---

### NewSnapshot
```
void NewSnapshot(string name);
```

Creates a new snapshot.

##### Arguments:
* name - Name of snapshot to create.

---

### RemoveSnapshot
```
void RemoveSnapshot(string name);
```

Removes a snapshot from VM.

##### Arguments:
* name - Name of snapshot to remove.

---

### RevertToSnapshot
```
void RevertToSnapshot(string name);
```

Reverts VM to target snapshot.

##### Arguments:
* name - Name of snapshot to revert to.

### ShowUI
```
void ShowUI();
```

Shows the UI for the VM.

##### Arguments:
None

---

### SetCredentials
```
void SetCredentials(string group);
```

Sets the current credentials to be used for all of the actions on this interface.

By default the admin group is used.

##### Arguments:
* group - Name of group to use.

---

### FileExistsInGuest
```
bool FileExistsInGuest(string path);
```

Determines if file exists in guest VM.

##### Arguments:
* path - Path of file in guest to check.

---

### PowerState
```
VMPower PowerState { get; }
```

Gets the current power state of the virtual machine. Values are as follows:

```
public enum VMPower
{
    Ready,
    Pending,
    Off
}
```

---

### OS
```
GuestOS OS { get; }
```

Gets the guest operating system of the VM.

Possible values are as follows:

```
public enum GuestOS
{
    Nano,
    Windows2016,
    Windows2016Core,
    Windows2012R2,
    Windows2012R2Core,
    Windows2012,
    Windows2012Core,
    Windows2008R2,
    Windows2008R2Core,
    Windows10,
    Windows81,
    Windows8,
    Windows7,
    Ubuntu
}
```

### Arch
```
Arch Arch { get; }
```

Gets the architecture of the operating system.

Possible values are as follows:
```
public enum Arch
{
    X64,
    X86
}
```

---

### Name
```
string Name { get; }
```

Gets name of the VM specified in the vmlab.csx file.

---

### Memory
```
int Memory { get; }
```

Gets the memory of the VM specified in vmlab.csx file.

---

### Cpu
```
int Cpu { get; }
```

Gets the number of CPUs in VM specified in vmlab.csx file.

---

## CpuCore
```
int CpuCore { get; }
```

Gets the number of CPUCores in VM specified in bmlab.csx file.

