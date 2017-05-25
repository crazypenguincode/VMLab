using System;
using System.Collections.Generic;
using VMLab.GraphModels;

namespace VMLab.Contract.OSEnvironment
{
    public class WindowsEnvironment : IOSEnvironment
    {
        public virtual IEnumerable<GuestOS> SupportedOS => new[]
        {
            GuestOS.Nano,
            GuestOS.Windows2016,
            GuestOS.Windows2016Core,
            GuestOS.Windows2012R2,
            GuestOS.Windows2012R2Core,
            GuestOS.Windows2012,
            GuestOS.Windows2012Core,
            GuestOS.Windows2008R2,
            GuestOS.Windows2008R2Core,
            GuestOS.Windows10,
            GuestOS.Windows81,
            GuestOS.Windows8,
            GuestOS.Windows7
        };

        public virtual IEnumerable<Arch> SupportedArch => new[] { Arch.X64, Arch.X86 };
        
        public virtual int Priority => 100;
        public virtual string TempDirectory => "c:\\Windows\\Temp\\";
        public virtual string PowershellExe => "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
        public virtual string PowershellArgs => "-executionpolicy bypass -noprofile -noninteractive -file";
        public virtual string Shell => "c:\\windows\\system32\\cmd.exe";
        public virtual string ShellPreArg => "/c ";
        public virtual string LinkCommand => "mklink /D \"$$GuestPath$$\" \"\\\\vmware-host\\Shared Folders\\$$Name$$\"";
        public virtual string RemoveDirectory => "rd \"$$Folder$$\" /s /q";
        public virtual string ShellScriptExtension => "cmd";

        public virtual IEnumerable<string> RunScript => new[]
        {
            "@echo off",
            "\"$$path$$\" $$args$$ > \"$$stdout$$\" 2> \"$$stderr$$\"",
            "echo %ERRORLEVEL% > \"$$RETFILE$$\""
        };

        public string NewLine => Environment.NewLine;
    }
}