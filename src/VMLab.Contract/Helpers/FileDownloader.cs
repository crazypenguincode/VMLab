using System;
using System.Net;
using System.Threading;
using VMLab.Helper;

namespace VMLab.Contract.Helpers
{
    public class FileDownloader : IFileDownloader
    {
        private bool _finished = false;
        private int _lastpercent = -1;

        private readonly IConsole _console;

        public FileDownloader(IConsole console)
        {
            _console = console;
        }

        public void DownloadFile(string uri, string path)
        {
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
