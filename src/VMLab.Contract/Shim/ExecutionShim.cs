using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SystemInterface.IO;
using VMLab.Contract.Helpers;
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
        private readonly IRetryHelper _retryHelper;


        public ExecutionShim(IFile file, IConsole console, IConfig config, IRetryHelper retryHelper)
        {
            _file = file;
            _console = console;
            _config = config;
            _retryHelper = retryHelper;
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
            var tries = 10;

            while (!FileExist(stdret) || firstRun)
            {
                try
                {
                    firstRun = false;
                    Thread.Sleep(1000);

                    var outtext = GetFileAction(stdout).Skip(outindex).ToArray();
                    var errtext = GetFileAction(stderr).Skip(errindex).ToArray();

                    outindex += outtext.Length;
                    errindex += errtext.Length;

                    foreach (var line in outtext)
                        _console.Information(line);

                    foreach (var line in errtext)
                        _console.Error(line);

                    tries = 10;
                }
                catch(Exception e)
                {
                    if (tries < 1)
                    {
                        _console.Information("Script Terminated: Connection lost...");
                        _console.Error(e, "Error: {e}", e);
                        break;
                    }

                    _console.Warning("Having trouble reading text... retrying.");
                    Thread.Sleep(5000);
                    tries--;
                }
            }
            try
            {
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
            catch
            {
                return -1;
            }
        }


        public IOSEnvironment OSEnvironment { get; set; }
    }
}
