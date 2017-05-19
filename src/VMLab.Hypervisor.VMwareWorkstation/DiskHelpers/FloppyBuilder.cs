using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils;
using DiscUtils.Fat;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.VMwareWorkstation.DiskHelpers
{
    public class FloppyBuilder  : IFloppyBuilder
    {
        public void Build(string path, IEnumerable<FloppyFile> files)
        {
            using (var image = File.Create(path))

                
            {
                using (var floppy = FatFileSystem.FormatFloppy(image, FloppyDiskType.HighDensity, "unattend"))
                {
                    foreach (var file in files)
                    {
                        var filestream = floppy.OpenFile(file.FileName, FileMode.Create);
                        var source = File.OpenRead(file.SourcePath);
                        var buffer = new byte[source.Length];
                        source.Read(buffer, 0, buffer.Length);
                        filestream.Write(buffer, 0, buffer.Length);
                        source.Close();
                        filestream.Close();
                    }
                }
            }
        }
    }
}
