namespace VMLab.CommandHandler
{
    public interface IParamHandler
    {
        bool RootCommand { get; }
        bool CanHandle(string[] args);
        void Handle(string[] args);
    }
}
