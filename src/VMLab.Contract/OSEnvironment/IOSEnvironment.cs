using System.Collections.Generic;
using VMLab.GraphModels;

namespace VMLab.Contract.OSEnvironment
{
    public interface IOSEnvironment
    {
        IEnumerable<GuestOS> SupportedOS { get; }
        IEnumerable<Arch> SupportedArch { get; }
        int Priority { get; }
        string TempDirectory { get; }
        string PowershellExe { get; }
        string PowershellArgs { get; }
        string Shell { get; }
        string ShellPreArg { get; }
        string LinkCommand { get; }
        string RemoveDirectory { get; }
        string ShellScriptExtension { get; }
        IEnumerable<string> RunScript { get; }
        string NewLine { get; }
    }
}
