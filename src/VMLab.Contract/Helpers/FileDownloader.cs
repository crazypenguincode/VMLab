using System;
using System.IO;
using System.Net;
using System.Threading;
using Serilog;
using VMLab.Helper;

namespace VMLab.Contract.Helpers
{
    public class FileDownloader : IFileDownloader
    {
        private bool _finished = false;
        private int _lastpercent = -1;

        private readonly IConsole _console;
        private readonly ILogger _log;

        public FileDownloader(IConsole console, ILogger log)
        {
            _console = console;
            _log = log;
        }

        public void DownloadFile(string uri, string path)
        {
            _log.Information("Downloading file {file} from {url}", path, uri);

            var directory = Path.GetDirectoryName(path);

            if(directory == null)
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var web = new WebClient())
            {
                web.DownloadProgressChanged += WebOnDownloadProgressChanged;
                web.DownloadFileCompleted += Web_DownloadFileCompleted;
                web.DownloadFileAsync(new Uri(uri), path);

                while (!_finished)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void Web_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _finished = true;
        }

        private void WebOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            if (downloadProgressChangedEventArgs.ProgressPercentage <= _lastpercent) return;

            _lastpercent = downloadProgressChangedEventArgs.ProgressPercentage;
            _console.Information("Download Progress: {percent}%", _lastpercent);
        }
    }
}
