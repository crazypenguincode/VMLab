﻿using System;
using SystemInterface;
using SystemInterface.IO;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Hypervisor.VMwareWorkstation.DiskHelpers;
using VMLab.Hypervisor.VMwareWorkstation.VMX;

namespace VMLab.Hypervisor.VMwareWorkstation
{
    public class VMBuilder : IVMBuilder
    {
        private readonly Func<IVMXCollection> _vmxFactory;
        private readonly IHardDriveBuilder _driveBuilder;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IPVNHelper _ipvnHelper;
        private readonly IGuestOSTranslator _osTranslator;
        private readonly IEnvironment _environment;

        public VMBuilder(Func<IVMXCollection> vmxFactory, IHardDriveBuilder driveBuilder, IDirectory directory,  IPVNHelper ipvnHelper, IGuestOSTranslator osTranslator, IEnvironment environment, IFile file)
        {
            _vmxFactory = vmxFactory;
            _driveBuilder = driveBuilder;
            _directory = directory;
            _ipvnHelper = ipvnHelper;
            _osTranslator = osTranslator;
            _environment = environment;
            _file = file;
        }

        public bool CanBuild(Template template)
        {
            if (template.CPUCores < 1)
                return false;
            

            if (template.CPUs < 1)
                return false;

            if (template.HardDisks.Count > 16)
                return false;

            return template.Memory >= 1;
        }

        public void Build(Template template, string templateFolder)
        {
            var vmx = _vmxFactory();

            if (!_directory.Exists(templateFolder))
                _directory.CreateDirectory(templateFolder);

            vmx.WriteValue(".encoding", "windows-1252");
            vmx.WriteValue("config.version", "8");
            vmx.WriteValue("virtualHW.version", "12");
            vmx.WriteValue("memsize", template.Memory.ToString());
            vmx.WriteValue("mem.hotadd", "true");
            vmx.WriteValue("numvcpus", (template.CPUCores * template.CPUs).ToString());
            vmx.WriteValue("cpuid.coresPerSocket", template.CPUCores.ToString());
            vmx.WriteValue("scsi0.present", "TRUE");
            vmx.WriteValue("scsi0.virtualDev", "lsisas1068");
            vmx.WriteValue("sata0.present", "TRUE");
            vmx.WriteValue("displayName", template.Name);
            vmx.WriteValue("guestOS", _osTranslator.FromVMLabGuestOS(template.GuestOS, template.Arch));
            vmx.WriteValue("powerType.powerOff", "soft");
            vmx.WriteValue("powerType.powerOn", "soft");
            vmx.WriteValue("powerType.suspend", "soft");
            vmx.WriteValue("powerType.reset", "soft");
            vmx.WriteValue("sound.present", "TRUE");
            vmx.WriteValue("sound.virtualDev", "hdaudio");
            vmx.WriteValue("sound.fileName", "-1");
            vmx.WriteValue("sound.autodetect", "TRUE");
            vmx.WriteValue("pciBridge0.present", "TRUE");
            vmx.WriteValue("pciBridge4.present", "TRUE");
            vmx.WriteValue("pciBridge4.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge4.functions", "8");
            vmx.WriteValue("pciBridge5.present", "TRUE");
            vmx.WriteValue("pciBridge5.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge5.functions", "8");
            vmx.WriteValue("pciBridge6.present", "TRUE");
            vmx.WriteValue("pciBridge6.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge6.functions", "8");
            vmx.WriteValue("pciBridge7.present", "TRUE");
            vmx.WriteValue("pciBridge7.virtualDev", "pcieRootPort");
            vmx.WriteValue("pciBridge7.functions", "8");
            vmx.WriteValue("bios.bootOrder", "cdrom, hdd, floppy");

            var index = 0;

            if (template.ISO != null)
            {
                vmx.WriteValue("sata0:1.present", "TRUE");
                vmx.WriteValue("sata0:1.fileName", _environment.CurrentDirectory + "\\" + template.ISO.LocalPath);
                vmx.WriteValue("sata0:1.deviceType", "cdrom-image");
            }

            foreach (var disk in template.HardDisks)
            {
                vmx.WriteValue($"scsi0:{index}.present", "TRUE");
                vmx.WriteValue($"scsi0:{index}.fileName", $"disk{index}.vmdk");

                var sizeinBytes = (long)disk.Size * 1024 * 1024 * 1024; // Converting GiB into Bytes
                _driveBuilder.BuildDrive($"{templateFolder}\\disk{index}.vmdk", sizeinBytes); 
                index++;
            }

            if (!string.IsNullOrEmpty(template.FloppyImage))
            {
                vmx.WriteValue("floppy0.fileType", "file");
                vmx.WriteValue("floppy0.fileName", "autoinst.flp");
                vmx.WriteValue("floppy0.clientDevice", "FALSE");

                _file.Copy(template.FloppyImage, $"{templateFolder}\\autoinst.flp");
            }

            index = 0;

            if (template.Networks.Count <= 0) return;

            foreach (var nic in template.Networks)
            {
                vmx.WriteValue($"ethernet{index}.present", "TRUE");

                switch (nic.Type)
                {
                    case NetworkType.Bridged:
                        vmx.WriteValue($"ethernet{index}.connectionType", "bridged");
                        break;
                    case NetworkType.NAT:
                        vmx.WriteValue($"ethernet{index}.connectionType", "nat");
                        break;
                    case NetworkType.Private:
                        vmx.WriteValue($"ethernet{index}.connectionType", "pvn");
                        vmx.WriteValue($"ethernet{index}.pvnID", _ipvnHelper.GetPVN(nic.Name));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if(template.GuestOS == GuestOS.Windows7 || template.GuestOS == GuestOS.Windows2008R2 || template.GuestOS == GuestOS.Windows2008R2Core)
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000");
                else
                    vmx.WriteValue($"ethernet{index}.virtualDev", "e1000e");

                vmx.WriteValue($"ethernet{index}.wakeOnPcktRcv", "FALSE");
                vmx.WriteValue($"ethernet{index}.addressType", "generated");
            }

            vmx.WriteToFile($"{templateFolder}\\{template.Name}.vmx");
        }
    }
}
