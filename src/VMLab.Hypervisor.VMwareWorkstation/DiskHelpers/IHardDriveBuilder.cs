﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Hypervisor.VMwareWorkstation.DiskHelpers
{
    public interface IHardDriveBuilder
    {
        void BuildDrive(string path, long size);
    }
}
