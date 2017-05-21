using System;
using System.Collections.Generic;

namespace VMLab.Contract.Shim
{
    public interface IExecutionShim
    {
        Func<string, IEnumerable<string>> GetFileAction { set; }
        Action<string, string> PutFileAction { set; }
        Action<string, string> ExecutionAction { set; }
        Action<string> RemoveFile { get; set; }
        Func<string, bool> FileExist { get; set; }
        int Execute(string path, string args);

    }
}
