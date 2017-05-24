namespace VMLab.Script.FluentInterface
{
    public interface IExecResult
    {
        void Fail(string message = "Bad return code!");
        void Success(string message = "Command ran successfully!");
        bool Retry { get; set; }
        int ReturnCode { get; }
    }
}
