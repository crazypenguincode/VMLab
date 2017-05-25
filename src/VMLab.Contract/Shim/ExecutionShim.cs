using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SystemInterface.IO;
using VMLab.Contract.OSEnvironment;
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

            var localrunfile = $"{_config.GetSetting("TempDir")}\\{id}.{OSEnvironment.ShellScriptExtension}";
            var guestrunfile = $"{OSEnvironment.TempDirectory}{id}.{OSEnvironment.ShellScriptExtension}";
            var stdout = $"{OSEnvironment.TempDirectory}{id}.stdout";
            var stderr = $"{OSEnvironment.TempDirectory}{id}.stderr";
            var stdret = $"{OSEnvironment.TempDirectory}{id}.stdret";

            var runfile = new StringBuilder();
            foreach (var line in OSEnvironment.RunScript)
                runfile.AppendLine(line.Replace("$$path$$", path)
                    .Replace("$$args$$", args)
                    .Replace("$$stdout$$", stdout)
                    .Replace("$$stderr$$", stderr)
                    .Replace("$$RETFILE$$", stdret));


            _file.WriteAllText(localrunfile, runfile.ToString().Replace(Environment.NewLine, OSEnvironment.NewLine));

            PutFileAction(localrunfile, guestrunfile);
            _file.Delete(localrunfile);

            ExecutionAction(OSEnvironment.Shell, $"{OSEnvironment.ShellPreArg}\"{guestrunfile}\"");

            var errindex = 0;
            var outindex = 0;
            var firstRun = true;

            while (!FileExist(stdret) || firstRun)
            {
                firstRun = false;
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

        public IOSEnvironment OSEnvironment { get; set; }
    }
}
