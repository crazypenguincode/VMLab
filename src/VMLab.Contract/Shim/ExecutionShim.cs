using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SystemInterface.IO;
using VMLab.Helper;

namespace VMLab.Contract.Shim
{
    public class ExecutionShim : IExecutionShim
    {
        public Func<string, IEnumerable<string>> GetFileAction { get; set; }
        public Action<string, string> PutFileAction { get; set; }
        public Action<string, string> ExecutionAction { get; set; }
        public Action<string> RemoveFile { get; set; }
        public Func<string, bool> FileExist { get; set; }

        private readonly IFile _file;
        private readonly IConfig _config;
        private readonly IConsole _console;


        public ExecutionShim(IFile file, IConsole console, IConfig config)
        {
            _file = file;
            _console = console;
            _config = config;
        }

        public int Execute(string path, string args)
        {
            var id = Guid.NewGuid().ToString();

            var localrunfile = $"{_config.GetSetting("TempDir")}\\{id}.cmd";
            var guestrunfile = $"c:\\windows\\temp\\{id}.cmd";
            var stdout = $"c:\\windows\\temp\\{id}.stdout";
            var stderr = $"c:\\windows\\temp\\{id}.stderr";
            var stdret = $"c:\\windows\\temp\\{id}.stdret";

            var runfile = new StringBuilder();
            runfile.AppendLine("@echo off");
            runfile.AppendLine($"\"{path}\" {args} > \"{stdout}\" 2> \"{stderr}\"");
            runfile.AppendLine($"echo %ERRORLEVEL% > \"{stdret}\"");

            _file.WriteAllText(localrunfile, runfile.ToString());

            PutFileAction(localrunfile, guestrunfile);
            _file.Delete(localrunfile);

            ExecutionAction("c:\\windows\\system32\\cmd.exe", $"/c \"{guestrunfile}\"");

            var errindex = 0;
            var outindex = 0;

            while (!FileExist(stdret))
            {
                Thread.Sleep(1000);

                var outtext = GetFileAction(stdout).Skip(outindex).ToArray();
                var errtext = GetFileAction(stderr).Skip(errindex).ToArray();

                outindex += outtext.Length;
                errindex += errtext.Length;

                foreach(var line in outtext)
                    _console.Information(line);

                foreach(var line in errtext)
                    _console.Error(line);
            }

            var result = GetFileAction(stdret).FirstOrDefault();

            if (result == null)
                return -1;

            if (FileExist(stdout))
                RemoveFile(stdout);
            if (FileExist(stderr))
                RemoveFile(stderr);
            if (FileExist(stdret))
                RemoveFile(stdret);

            return int.Parse(result);
        }
    }
}
