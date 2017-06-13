# Template
The template command gives you access to the ITemplate fluent interface.

## Example
Here is an example template deceleration and a example of how to call the fluent interface.
```
Template("Windows 10 x64", "0.1.0")
    .Credential("Admin", "administrator", "P@ssw0rd01")
    .ISO("ISO\\Windows10.iso", "https://webserver//Windows10.iso")
    .GuestOS("Windows10", "x64")
    .HardDisk(100)
    .Network("NAT")
    .CPU(1, 2)
    .Memory(2048)
    .FloppyImage("floppy.flp");
```

## Methods
Here are all the methods supported by the ITemplate fluent interface.

```
    public interface ITemplate
    {
        string Name { get; set; }
        string Version { get; set; }

        ITemplate Credential(string group, string username, string password);
        ITemplate ISO(string path, string url = null);
        ITemplate GuestOS(string os, string arch);
        ITemplate HardDisk(int size);
        ITemplate Network(string type, string name="");
        ITemplate CPU(int cpus, int cores);
        ITemplate Memory(int size);
        ITemplate FloppyImage(string path);
        ITemplate WithHypervisor(string hypervisor, Action<ITemplate> action);
        ITemplate OnProvision(Action<IVMControl> action);
        ITemplate Healdess(bool headless);
    }
```

---

### Credential
```
ITemplate Credential(string group, string username, string password);
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
Template("Windows 10 x64", "0.1.0")
    .Credential("Admin", "administrator", "P@ssw0rd01");
```

---

### ISO
```
ITemplate ISO(string path, string url = null);
```

This method lets you specify an ISO file that is attached to the virtual machine to build it. It is also possible to pass in a URL to the iso file so it can be downloaded if required.

##### Arguments:
* path - Path to the ISO file or where it should be saved to once downloaded.
* url - optional url to where to download the ISO file from.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .ISO("ISO\\Windows10.iso", "https://webserver//Windows10.iso");
```

---

### GuestOS
```
ITemplate GuestOS(string os, string arch);
```

This specifies the guest operating system being installed in the Virtual Machine. This is important as it will select the correct hardware settings for the virtual machine and control how things behave.

##### Arguments:
* os - This is the operating system to be installed. See list below for supported options.
* arch - This is the architecture to install. This can be x86 or x64.

##### Supported Operating Systems:
* windows2016
* windows2016core
* windows2012r2
* windows2012r2core
* windows2012
* windows2012core
* windows2008r2
* windows2008r2core
* windows10
* windows81
* windows8
* windows7

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .GuestOS("Windows10", "x64");
```

---

### HardDisk
```
ITemplate HardDisk(int size);
```

Adds a hard disk to the vm of target size.

##### Arguments:
* size - The size in GB that you want the hard drive to be. This drive will be provisioned in sparse mode if the hypervisor supports it to save space.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .HardDisk(100);
```

---

### Network
```
ITemplate Network(string type, string name="");
```

This adds a network interface to the vm allowing it to access external networks or internal lab only networks.

##### Arguments:
* type - This is the type of network to connect to. See the list below for allowed network types.
* name - This is the name of the network to connect to. You only need to use this for private networks.

##### Network Types
* bridged - This will connect the VM to the same network as the host laptop.
* private - This will connect the VM to a private network that only other VMs with the same network name can see. Useful for creating a lab that you don't want interfering with outside networks.
* nat - This will connect the VM to a NAT network. Which allows it to see the outside network (and internet) but doesn't allow external machines to see its network. This is the most command network type that can be used to let a VM see the internet. For template generation this network is recommended.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .Network("NAT");
```

---

### CPU
```
ITemplate CPU(int cpus, int cores);
```

This specifies how many cores and cpus the VM has.

##### Arguments:
* cpus - How many cpus the VM has.
* cores - How many cores each cpu will have.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .CPU(1, 2);
```

---

### Memory
```
ITemplate Memory(int size);
```

Specifies how much memory the VM has.

##### Arguments:
* size - How much ram a vm has in MB.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .Memory(2048);
```

---

### FloppyImage
```
ITemplate FloppyImage(string path);
```

This specifies a path to a floppy image to attach to the vm. This is useful for placing answer files to automate the install.

##### Arguments:
* path - path to the floppy image to attach.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .FloppyImage("floppy.flp");
```

---


### WithHypervisor
```
ITemplate WithHypervisor(string hypervisor, Action<ITemplate> action);
```

This method allows you to add configuration which is only specific to a specific hypervisor.

##### Arguments:
* hypervisor - The hypervisor to apply the configuration to use ```vmlab hypervisor list``` to see what values are valid.
* action - An action that contains the configuration to be applied if the current hypervisor is the one specified.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .WithHypervisor("Vmwareworkstation", c => {
        c.FloppyImage("vmware.flp");
    });
```

---

### OnProvision
```
ITemplate OnProvision(Action<IVMControl> action);
```

This method allows actions to be run on the VM after it is provisioned. This allows for operating system configuration.

This is run after the c:\vmlab.ready is created on the vm.

After this command is run the system is automatically shutdown and turned into a template.

It is recommended that windows templates are syspreped during the setup phase.

##### Arguments:
* action - The action to execute when the vm is ready for provision.

##### Example
```
Template("Windows 10 x64", "0.1.0")
    .OnProvision(vm => {
        vm.Powershell(".\\mysetupscript.ps1");
    });
```

---

### Headless
```
ITemplate Healdess(bool headless);
```

This method sets if the VM UI should be loaded during provisioning or not.

##### Arguments:
* headless - Set to false to show ui during build or true to hide it.


##### Example
```
Template("Windows 10 x64", "0.1.0")
    .Healdess(false);
```