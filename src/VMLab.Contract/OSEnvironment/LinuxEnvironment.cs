using System.Collections.Generic;
using VMLab.GraphModels;

namespace VMLab.Contract.OSEnvironment
{
    public class LinuxEnvironment : IOSEnvironment
    {
        public IEnumerable<GuestOS> SupportedOS => new[] { GuestOS.Ubuntu };
        public IEnumerable<Arch> SupportedArch => new[] { Arch.X64, Arch.X86 };
        public int Priority => 100;
        public string TempDirectory => "/tmp/";
        public string PowershellExe => "/usr/bin/powershell";
        public string PowershellArgs => "-file";
        public string Shell => "/bin/bash";
        public string ShellPreArg => "";
        public string LinkCommand => "ln -s /mnt/hgfs/$$Name$$ $$GuestPath$$";
        public string RemoveDirectory => "rd -rf $$folder$$";
        public string ShellScriptExtension => "sh";

        public IEnumerable<string> RunScript => new[]
        {
            "$$path$$ $$args$$ > $$stdout$$ 2> $$stderr$$",
            "echo $? > $$RETFILE$$"
        };

        public string NewLine => "\n";
    }
}
