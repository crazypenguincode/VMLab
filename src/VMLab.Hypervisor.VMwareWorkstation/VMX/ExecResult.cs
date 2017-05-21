using VMLab.Script.FluentInterface;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public class ExecResult : IExecResult
    {
        internal string FailMessage { get; set; }
        internal string SuccessMessage { get; set; }

        public void Fail(string message = "Bad return code!")
        {
            FailMessage = message;
        }

        public void Success(string message = "Command ran successfully!")
        {
            SuccessMessage = message;
        }

        public int ReturnCode { get; internal set; }
    }
}
