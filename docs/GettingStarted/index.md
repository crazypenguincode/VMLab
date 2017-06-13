# Getting Started
## Installation
Before you can do anything you need to get everything installed and ready to go.

* Install vmlab from [here](https://github.com/wiltaylor/VMLab/releases).
* Install VMware Workstation from [VMware.com](https://wwww.vmware.com). Please note: you must have a full version for this to work, demo versions do not work.

Note: In the future HyperV and VirtualBox will be supported. In the mean time only VMware Workstation is supported.

## Templates
Before you can create lab machines you need to create and import templates for use with VMLab.

There are some example templates you can use to get started in [this](https://github.com/wiltaylor/VMLab-WindowsTemplates) repository.

Read the readme of the repository for instructions on how to build and import them.

## First Lab Environment.
To create your first lab environment do the following:

* Create a empty folder where you want the lab environment to be (e.g. c:\mylab).
* From Powershell or cmd cd into the directory.
* Run the following command ``` vmlab init "Windows 10 x64" ```
* Run ``` vmlab start ```

Now you have a Windows 10 machine up and running ready to use for your development activities.

In the directory there will now be a vmlab.csx file which contains all the configuration of the VM(s) in the lab environment.

There is also a _vmlab folder which is used to store all the virtual machines during operation. You shouldn't need to manually do anything in this folder, vmlab will control it for you.

### Execute Commands
You can execute a command in the vm by running ``` vmlab exec myVM dir c:\``` which will display a directory listing of c:\ to the console.

You can run any other command this way and vmlab will run it on the vm using cmd.exe /c.

### Execute Powershell scripts.
You can execute any powershell script you have on you host machine by running ``` vmlab powershell myVM "c:\path\to\myscript.ps1" ``` and it will copy the script to the vm and run it.

### copying files between vm and host.
The default vmlab.csx file will have a shared folder created in it that maps the folder vmlab.csx is located in to c:\lab on the virtual machine. To copy files between the vm and the host it is just a matter of putting files in this directory.

### Stop, Restart VMs
You can control VMs by using ```vmlab stop``` and ```vmlab restart``` which will shutdown and restart virtual machines respectively.

### Cleanning up
After you have finished with the virtual machine and want to remove it you can do so by simply calling ```vmlab destroy``` this will as the name suggests will stop the vm and delete it from disk. 

You can recreate it again by simply running ```vmlab start```. This is useful for saving disk space when not using a virtual environment.

### Further reading
Now you understand the basics of VMLab you can read up further on the following topics:

* [VMLab.CSX file syntax.](../VMLabCSX/index.md)
* VMLab Command Line interface.

