namespace VMLab.Script
{
    public interface IScriptEngine
    {
        bool CanHandle { get; }
        void Execute();
    }
}
