# Command Line Interface
As VMLab is a command line tool it has a fairly extensive command line interface.

To use VMLab it is broken down into sub commands which can be operated like this:

vmlab [command] [subcommnd] (arguments) (switches) 

Available commands:

## Init 
```
vmlab init "templatename"
```

This command creates a lab environment by creating a vmlab.csx file in the current directory.

### Sub Commands:
None

### Arguments:
1 - Name of the template to use for default VM.

##### Switches:
None

---

## Start
```
vmlab start
```

This command will stop all VMs in a lab environment. If VM hasn't been created yet it will also create it.

### Sub Commands:
None

### Arguments:
None

##### Switches:
* vm - specify a list of VMs to start separated by spaces.

---

## Stop
```
vmlab stop
```

This command will stop all VMs in a lab environment.

### Sub Commands:
None

### Arguments:
None

##### Switches:
* vm - specify a list of VMs to stop separated by spaces.

---

## Restart
```
vmlab restart
```

This command will restart all VMs in a lab environment.

### Sub Commands:
None

### Arguments:
None

##### Switches:
* vm - specify a list of VMs to restart separated by spaces.

---

## Snapshot
```
vmlab snapshot
```

Handles the management of snapshots in the lab.

### Sub Commands:

#### List
```
vmlab snapshot list
```

Lists snapshots in lab.

##### Arguments:
None

##### Switches:
None

#### Add
```
vmlab snapshot add MySnapshot
```

Creates a new snapshot VMs in lab.

##### Arguments:
1 - Name of snapshot to create.

##### Switches:
* vm - specify a list of VMs to restrict this command to.

*** 

#### Remove
```
vmlab snapshot remove MySnapshot
```

Removes a snapshot from VMs in lab.

##### Arguments:
1 - Name of snapshot to remove.

##### Switches:
* vm - specify a list of VMs to restrict this command to.

*** 

#### Revert
```
vmlab snapshot revert MySnapshot
```

Revert machines in lab to specific snapshot.

##### Arguments:
1 - Name of snapshot to revert to.

##### Switches:
* vm - specify a list of VMs to restrict this command to.
---

## Version
```
vmlab version
```

This command will display the version of vmlab.

### Sub Commands:
None

### Arguments:
None

##### Switches:
None

---

## gui
```
vmlab gui
```

Shows the gui for VMs in the lab.

### Sub Commands:
None

### Arguments:
None

##### Switches:
* vm - specify a list of VMs to start separated by spaces.

---

## Exec
```
vmlab exec <vmname> <command>
```

Executes a command on the target vm.

### Sub Commands:
None

### Arguments:
1 - Name of VM to run command on.

2 - Command to run.

##### Switches:
None

---

## Powershell
```
vmlab powershell <vmname> <script path>
```
### Sub Commands:
None

### Arguments:
1 - Name of VM to run command on.

2 - Path to powershell script to run.

##### Switches:
None

---

## Status
```
vmlab status
```

Lists the status of VMs in lab.

### Sub Commands:
None

### Arguments:
None

##### Switches:
None

---

## hypervisor
```
vmlab hypervisor
```

Hypervisor command allows the user to list or set the hypervisor to use.

### Sub Commands:

#### List
```
vmlab hypervisor list
```

Lists all hypervisors that vmlab supports.

##### Arguments:
None

###### Switches:
None

***
#### Set
```
vmlab hypervisor set "Vmwareworkstation"
```

Sets the hypervisor for vmlab to use.

##### Arguments:
1 - Name of hypervisor to use.

###### Switches:
None

---

## Template
```
vmlab template
```

Allows the user interact with templates.

### Sub Commands:

#### Build
````
vmlab template build
````

Builds template(s) specified by the vmlab.csx file in the current directory.

##### Arguments:
None

###### Switches:
None

****

#### list
```
vmlab template list
```

Lists all the templates currently imported into vmlab.

##### Arguments:
None

###### Switches:
None

***

#### Import
```
vmlab template import <path>
```

Imports a template into vmlab ready for use in vmlab.csx files.

##### Arguments:
1 - Path to template file to import.

###### Switches:
None

***

#### Remove
```
vmlab template remove <name>
```

Removes a template from vmlab.

##### Arguments:
1 - Name of template to remove.

###### Switches:
None

---

## Lab
```
vmlab lab
```

Allows import, export and clean of lab environment.

### Sub Commands:

#### export
```
vmlab lab export <path>
```

Exports the current lab environment to an archive to be imported later on.

This is useful for when you have setup a lab manually and want to restore the exact state or if you need to migrate it from one machine to another.

Note: Running this command will generally convert linked clones into full clones which will result in more disk space being used.

##### Arguments:
1 - Path to where export file is to be created.

###### Switches:
None

***

#### import
```
vmlab lab import <path>
```

Imports an exported lab into the current directory.

##### Arguments:
1 - Path to where export file is located.

##### Switches:
None

***

#### clean
```
vmlab lab clean
```

Clears all lab files created by vmlab by deleting _vmlab folder. 



## Action
```
vmlab action <actionName> <action arguments>
```

Runs custom action defined in vmlab.csx file.

### Sub Commands:
None

### Arguments:
1 - Name of action to run.

2 - Arguments passed to action.

### Switches:
None

---

## Credential
```
vmlab credential
```

Manages credentials for the lab environment.

### Sub Commands:

#### List
```
vmlab credential list
```

Lists all credentials used in lab environment. This include both credentials assigned in vmlab.csx and secure stored credentials. This does not show the passwords.

##### Arguments:
None

##### Switches:
None

***

#### Add
```
vmlab credential add -vm <vmname> -group <groupname> -username <username> -password <password>
```

Adds a secure credential to the lab.

##### Arguments:
None

##### Switches:
* vm - Name of VM to assign credentails to.
* group - Name of group to assign credentials to.
* username - Username of the credential.
* password - Password of the credential. If this switch is omitted you will be prompted to enter the password.

***

#### Remove
```
vmlab credential remove -vm <vmname> -group <groupname>
```

Removes a secure credential from the lab.

##### Arguments:
* vm - Name of VM to remove the credential from.
* group - Name of group to remove.

***

#### Clear
```
vmlab credential clear
```

Removes all secure credentials from lab.

##### Arguments:
None

##### Switches
* force - Clears credentials without prompting user to confirm.


## Config
```
vmlab config
```

The ability to change or view vmlab configuration.

### Sub Commands:

#### Get
```
vmlab config get <level> <valuename>
```

##### Arguments:
1 - Config level. See table below.
2 - Name of configuration value to retrieve.

##### Switches
None

##### Config levels
* system - Highest level of configuration, this is the same for all users who logon to this machine.
* user - This is a per user level configuration item.
* lab - This is per lab level of configuration. This is stored in _vmlab folder.
* merged - This is a merged configuration of the above settings. user overrides system and lab overrides user.

***

#### Set
```
vmlab config set <level> <valuename> <value>
```
##### Arguments:
1 - Config level. See table below.
2 - Name of configuration value to change.
3 - Value to change value to.

##### Switches
None

##### Config levels
* system - Highest level of configuration, this is the same for all users who logon to this machine.
* user - This is a per user level configuration item.
* lab - This is per lab level of configuration. This is stored in _vmlab folder.

Note: You can't set merged level as its not a real level.

***

#### View
```
vmlab config view <level>
```

Shows all configuration for a sepcified level.

##### Arguments:
1 - Config level. See table below.

##### Switches
None

##### Config levels
* system - Highest level of configuration, this is the same for all users who logon to this machine.
* user - This is a per user level configuration item.
* lab - This is per lab level of configuration. This is stored in _vmlab folder.



