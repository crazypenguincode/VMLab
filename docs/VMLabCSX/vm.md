# VM
The VM command gives you access to the IVM fluent interface.

## Example
```
VM("myVM")
	.Template("Windows 10 x64")
	.Credential("Admin", "Administrator", "P@ssw0rd01")
	.Network("NAT")
	.CPU(1,2)
	.Memory(2048)
	.ShareFolder(".", "c:\\lab");
```

## Methods
Here are all the methods supported by the ITemplate fluent interface.

```
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
        IVM Property(string name, string value);
        IVM NestedVirtualization();
    }
```

---

### Template
```
IVM Template(string name, string version = "latest");
```

This specifies the template to use when provisioning this vm.

##### Arguments:
* name - Name of the template to use for this vm. (e.g. Windows10x64).
* version - Optional parameter with the version of the template to use. If omitted the template with the highest version number will be used.

##### Example
```
VM("myVM")
	.Template("Windows 10 x64");
```

---

### Credential
```
IVM Credential(string group, string username, string password);
```

This method allows you to set a credential for use in the lab. This can be useful for having different credentials for a VM after it has joined a domain or if you want to have user credentials and admin credentials for testing.

You can specify which credentials are being used by using the SetCredentials method of a vm object.

Note on Security:
You should not use any production passwords for your VMs. It is recommended you use temp passwords like P@ssw0rd01.

If you must use production passwords and want to keep them safe see credential command line option which allows you to store secure passwords.

##### Arguments:
* group - This is the name of the group that the credentials belong to. Currently you can only have one set of credentials in a group.
* username - The username of the credential. This can include the domain too (e.g. mydomain\myusername).
* password - Plain text password to use.

##### Example

```
VM("myVM")
    .Credential("Admin", "administrator", "P@ssw0rd01");
```

---

### Network
```
IVM Network(string type, string name="");
```

This adds a network interface to the vm allowing it to access external networks or internal lab only networks.

##### Arguments:
* type - This is the type of network to connect to. See the list below for allowed network types.
* name - This is the name of the network to connect to. You only need to use this for private networks.

##### Network Types
* bridged - This will connect the VM to the same network as the host laptop.
* private - This will connect the VM to a private network that only other VMs with the same network name can see. Useful for creating a lab that you don't want interfering with outside networks.
* nat - This will connect the VM to a NAT network. Which allows it to see the outside network (and internet) but doesn't allow external machines to see its network. This is the most command network type that can be used to let a VM see the internet.

##### Example
```
VM("myVM")
    .Network("NAT");
```

---

### CPU
```
IVM CPU(int cpus, int cores);
```

This specifies how many cores and cpus the VM has.

##### Arguments:
* cpus - How many cpus the VM has.
* cores - How many cores each cpu will have.

##### Example
```
VM("myVM")
    .CPU(1, 2);
```

---

### Memory
```
IVM Memory(int size);
```

Specifies how much memory the VM has.

##### Arguments:
* size - How much ram a vm has in MB.

##### Example
```
VM("myVM")
    .Memory(2048);
```

---

### SharedFolder
```
IVM ShareFolder(string hostpath, string guestpath);
```

This method lets you specify a shared folder. A shared folder is a folder on the host that is linked to a folder in the guest VM. This allows the sharing of files between both environments and is really useful for development work.

##### Arguments:
* hostpath - Path on the Host operating system to link.
* guestpath - Path inside the guest VM to link to.

### WithHypervisor
```
IVM WithHypervisor(string hypervisor, Action<IVM> action);
```

This method allows you to add configuration which is only specific to a specific hypervisor.

##### Arguments:
* hypervisor - The hypervisor to apply the configuration to use ```vmlab hypervisor list``` to see what values are valid.
* action - An action that contains the configuration to be applied if the current hypervisor is the one specified.

##### Example
```
VM("myVM")
    .WithHypervisor("Vmwareworkstation", c => {
        c.FloppyImage("vmware.flp");
    });
```

---

### OnProvision
```
IVM OnProvision(Action<IVMControl> action);
```

This method allows actions to be run on the VM after it is first created. This allows for operating system configuration.

This is run after the vm has started up and the c:\\provision.wait no longer exists.

This will only be run the first time a VM is started or if a vm is started after it has been previously destroyed with ```vmlab destroy```.

##### Arguments:
* action - The action to execute when the vm is ready for provision.

##### Example
```
VM("myVM")
    .OnProvision(vm => {
        vm.Powershell(".\\mysetupscript.ps1");
    });
```

---

### OnDestroy
```
IVM OnDestroy(Action<IVMControl> action);
```

This method allows actions to be on a VM before it is destroyed.

##### Arguments:
* action - The action to execute when the vm is ready for provision.

##### Example
```
VM("myVM")
    .OnDestroy(vm => {
        vm.Powershell(".\\cleanup.ps1");
    });
```

---

### AfterDestroy
```
IVM AfterDestroy(Action action);
```

This method allows actions to be run after a VM has been deleted from disk. Useful for further clean up.

##### Arguments:
* action - The action to execute when the vm is ready for provision.

##### Example
```
VM("myVM")
    .AfterDestroy(vm => {
        //Do clean up here.
    });
```

---

### NestedVirtualization
```
IVM NestedVirtualization();
```

This enables Nested Virtualization support in the VM. This is useful if you want to run another hypervisor inside the VM. Not all Hypervisors support this.

##### Arguments:
None

##### Example
```
VM("myVM")
    .NestedVirtualization();
```