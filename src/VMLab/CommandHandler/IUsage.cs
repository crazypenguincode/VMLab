namespace VMLab.CommandHandler
{
    public interface IUsage
    {
        void WriteUsage(IParamHandler[] handlers);
        void WriteHubUsage(IParamHandler hub, IParamHandler[] handlers);
        void WriteCommandUsage(IParamHandler handler);
    }
}
