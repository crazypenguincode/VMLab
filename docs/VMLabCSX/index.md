# VMLab.CSX File
This file is the heart of VMLab and is used to describe a Lab environment or a Template to be built.

## File Syntax
The vmlab.csx file is actually a C# Roslyn script file which contains configuration of how you want the VM environment to be laid out. This gives you a lot of flexibility but you don't need to know all the advanced syntax of C# to create a vmlab.csx file.

## Methods
At its core vmlab supports the following methods (or functions) that can be called to do actions.

---

### Echo
``` 
void Echo(string text); 
```

Simple command used to write text to the console to notify user of something.

##### Arguments:
* text - Writes a message to the console window.

---

### Pause
```
void Pause(string text);
```

Pauses execution of script and prompts to press any key to continue.

##### Arguments:
* text - Writes a message to the console window.

---

### Template
```
ITemplate Template(string name, string version);
```

Describe a template to be created when the user runs ```vmlab template build```. This method returns a ITemplate object which supplies a fluent interface that allows you to configure more options of the template.

For more information on how to create templates see [this](template.md).

---

##### Arguments:
* name - The name of the template. For example "Windows 10 x64"
* version - A version number for the template. This follows symantic versioning (e.g. 0.1.0).

### VM
```
IVM VM(string name);
```

Describes a VM to create in the lab environment that is created when the user runs ```vmlab start```. This method returns a IVM object which supplies a fluent interface that allows you to configure more options of the vm.

For more information on how to create VMs see [this](vm.md).

##### Example
```
VM("myVM")
	.Template("Windows 10 x64")
	.Credential("Admin", "Administrator", "P@ssw0rd01")
	.Network("NAT")
	.CPU(1,2)
	.Memory(2048)
	.ShareFolder(".", "c:\\lab");
```

---

##### Arguments:
* name - Name of the virtual machine. This is used when targeting the vm with vmlab commands. This is not the host name of the machine, this needs to be changed manually with configuration scripts.

### Lab
```
void Lab(string name, string author, string description);
```

This command lets you specify lab meta data of a lab which can be viewed by running ```vmlab status```.

##### Arguments:
* name - Name of the lab environment.
* author - Name of the author of the lab environment.
* Description - A description of the lab environment.

##### Example
```
Lab("MyLab", "Wil Taylor", "Demo example lab.");
```

---

### LockAction
```
void LockAction(string action);
```

Used to prevent specified action from running against the lab.

##### Arguments:
* action - action to prevent from running. See list below for actions which can be blocked.

##### Actions
* destroy - prevent the user from destroying lab. Useful if lab was manually built and you don't want to accidentally destroy it.

##### Example
```
LockAction("Destroy");
```

---

### Action
```
void Action(string name, Action<string[], ISession> action);
```

Allows the creation of a custom action which can be run by ```vmlab action <actionname>```.

This might look complicated for anyone who isn't a c# developer but it should be fairly straight forward once you have looked at some of the examples here.

##### Arguments:
* name - Name of the action that can be called from the commandline.
* action - This is a c# action which will be run the vmlab action is triggered.

##### Example
```
Action("MyAction", args => { 
    Echo("My Custom Action Ran"); 
});
```

---

### LoadJson
```
 Dictionary<string, string> LoadJson(string path);
```

Loads a json file into a Dictionary<string, string> object. This is useful for loading a json configuration file if you want to use parameters in your script that can be controlled from a json file.

##### Example
```
var settings = LoadJson(".\\mysettings.json");
Echo("Setting Name: " + settings["Name"]);
```