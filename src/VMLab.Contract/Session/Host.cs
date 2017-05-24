using System.Diagnostics;

namespace VMLab.Contract.Session
{
    public class Host : IHost
    {
        public int Exec(string path, string args, bool wait = true)
        {
            var proc = Process.Start(path, args);

            if (wait)
                proc?.WaitForExit();
            else
                return 0;

            return proc?.ExitCode ?? -1;
        }
    }
}
