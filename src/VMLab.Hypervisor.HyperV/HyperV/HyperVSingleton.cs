using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using SystemInterface.Threading;
using VMLab.Helper;
using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.HyperV.HyperV
{
    public class HyperVSingleton : IHyperV
    {
        private readonly PowerShell _powerShell;
        private readonly IConsole _console;
        private readonly IThread _thread;

        public HyperVSingleton(IConsole console, IThread thread)
        {
            _console = console;
            _thread = thread;
            _powerShell = PowerShell.Create();
            _powerShell.AddScript("Import-Module Hyper-V");
        }
        public void NewVM(string name, string path, int generation = 2)
        {

            var script = new StringBuilder();
            script.AppendLine($"$vm = New-VM -Name \"{name}\" -MemoryStartUpBytes 1024MB -Path \"{path}\" -Generation {generation}");

            DoPowershell(script.ToString());
        }

        public void SetCPUCount(string vmname, int count)
        {
            var script = new StringBuilder();
            script.AppendLine($"Set-VM -Name \"{vmname}\" -ProcessorCount {count}");

            DoPowershell(script.ToString());
        }

        public void SetMemory(string vmName, int memory, bool dynamic, int upper, int lower)
        {
            var script = new StringBuilder();
            script.AppendLine(dynamic
                ? $"Set-VM -Name \"{vmName}\" -DynamicMemory -MemoryStartupBytes {memory}MB -MemoryMaximumBytes {upper}MB -MemoryMinimumBytes {lower}MB"
                : $"Set-VM -Name \"{vmName}\" -StaticMemory -MemoryStartupBytes {memory}MB");
            DoPowershell(script.ToString());
        }

        public void AddDisk(string vmName, string path, int size, int location)
        {
            var script = new StringBuilder();
            script.AppendLine($"$vm = Get-VM -Name \"{vmName}\"");
            script.AppendLine($"$disk = New-VHD -Path \"{path}\" -Dynamic -SizeBytes {size}GB");
            script.AppendLine($"$vm | Add-VMHardDiskDrive -ControllerType SCSI -ControllerLocation {location} -Path \"{path}\"");
            DoPowershell(script.ToString());
        }

        public void AddDisk(string vmName, string path, int location)
        {
            var script = new StringBuilder();
            script.AppendLine($"$vm = Get-VM -Name \"{vmName}\"");
            script.AppendLine($"$vm | Add-VMHardDiskDrive -ControllerType SCSI -ControllerLocation {location} -Path \"{path}\"");
            DoPowershell(script.ToString());

        }

        public void SetFloppyImage(string vmName, string path)
        {
            var script = new StringBuilder();
            
            script.AppendLine($"Set-VMFloppyDiskDrive -VMName \"{vmName}\" -Path \"{path}\"");
            DoPowershell(script.ToString());
        }

        public void AddNetwork(string vmName, string vswitch)
        {
            var script = new StringBuilder();
            script.AppendLine($"Add-VMNetworkAdapter -VMName \"{vmName}\" -SwitchName \"{vswitch}\"");
            DoPowershell(script.ToString());
        }

        public void CreateDifferenceDisk(string source, string destination)
        {
            var script = new StringBuilder();
            script.AppendLine($"New-VHD -ParentPath \"{source}\" -Path \"{destination}\" -Differencing");
            DoPowershell(script.ToString());
        }

        public void ExecPowerShell(string vmname, string path)
        {
            var script = new StringBuilder();
            script.AppendLine($"Out-Default | Invoke-Command -VMName \"{vmname}\" -FilePath \"{path}\"");
            DoPowershell(script.ToString());
        }

        public void ExecuteCommand(string vmname, string path, string args, bool wait)
        {
            var script = new StringBuilder();

            script.AppendLine(wait
                ? $"Out-Default | Invoke-Command -VMName \"{vmname}\" -ScriptBlock {{ Start-Process -FilePath \"{path}\" -ArgumentList \"{args}\" -Wait }}"
                : $"Out-Default | Invoke-Command -VMName \"{vmname}\" -ScriptBlock {{ Start-Process -FilePath \"{path}\" -ArgumentList \"{args}\" }}");
            DoPowershell(script.ToString());
        }

        public bool FileExists(string vmName, string path)
        {
            return DoPowerShell<bool>($"Invoke-Command -VMName \"{vmName}\" -ScriptBlock {{ Test-Path \"{path}\"}}");
        }

        public void WaitReady(string vmname)
        {
            while (true)
            {
                try
                {
                    var count = DoPowerShell<int>($"Invoke-Command -VMName \"{vmname}\" -ScriptBlock {{ (Get-Process).Count }}");

                    if (count > 0)
                        break;

                    _thread.Sleep(3000);


                }
                catch
                {
                    //do nothing
                }
            }
        }

        public void Restart(string vmname, bool force)
        {
            var script = $"Restart-VM -Name \"{vmname}\"";

            if (force)
                script += " -Force";
            DoPowershell(script);
        }

        public void Stop(string vmname, bool force)
        {
            var script = $"Stop-VM -Name \"{vmname}\"";

            if (force)
                script += " -Force";
            DoPowershell(script);
        }

        public void Start(string vmname)
        {
            DoPowershell($"Start-VM -Name \"{vmname}\"");
        }

        public VMPower PowerState(string vmname)
        {
            var script = new StringBuilder();
            script.AppendLine($"$vm = get-vm -Name \"{vmname}\"");
            script.AppendLine("$vm.State.ToString()");

            var result = DoPowerShell<string>(script.ToString()).ToLower();

            switch (result)
            {
                case "running":
                    return VMPower.Ready;
                case "off":
                    return VMPower.Off;
                default:
                    return VMPower.Pending;
            }
        }

        public string[] GetSnapshot(string vmname)
        {
            return DoPowerShellAll<string>($"(Get-VMSnapshot -VMName \"{vmname}\").Name").ToArray();
        }

        public void CreateSnapshot(string vmname, string name)
        {
            DoPowershell($"Checkpoint-VM -Name \"{vmname}\" -SnapshotName \"{name}\"");
        }

        public void RevertSnapshot(string vmname, string name)
        {
            DoPowershell($"Restore-VMSnapshot -VMName \"{vmname}\" -Name \"{name}\"");
        }

        public void RemoveSnapshot(string vmname, string name)
        {
            DoPowershell($"Remove-VMSnapshot -VMName \"{vmname}\" -Name \"{name}\"");
        }

        public void DestroyVM(string vmName)
        {
            DoPowershell($"Remove-VM -VMName \"{vmName}\" -Force");
        }

        private T DoPowerShell<T>(string script)
        {
            _powerShell.AddScript(script);
            return _powerShell.Invoke().Cast<T>().FirstOrDefault();
        }

        private IEnumerable<T> DoPowerShellAll<T>(string script)
        {
            _powerShell.AddScript(script);
            return _powerShell.Invoke().Cast<T>();
        }

        private void DoPowershell(string script)
        {
            _powerShell.AddScript(script);
            _powerShell.Invoke();

            foreach (var record in _powerShell.Streams.Information)
            {
                _console.Information("{message}", record.MessageData);
            }

            foreach (var record in _powerShell.Streams.Error)
            {
                _console.Error("{@message}", record.ErrorDetails);
            }

            foreach (var record in _powerShell.Streams.Warning)
            {
                _console.Warning("{message}", record.Message);   
            }

            _powerShell.Streams.ClearStreams();

        }
    }
}
