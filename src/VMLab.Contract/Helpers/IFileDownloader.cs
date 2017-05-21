namespace VMLab.Contract.Helpers
{
    public interface IFileDownloader
    {
        void DownloadFile(string uri, string path);
    }
}
