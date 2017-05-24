namespace VMLab.Contract.Session
{
    public interface IHost
    {
        int Exec(string path, string args, bool wait = true);
    }
}
