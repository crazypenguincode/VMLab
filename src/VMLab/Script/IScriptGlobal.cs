using System;
using VMLab.Script.FluentInterface;

namespace VMLab.Script
{
    public interface IScriptGlobal
    {
        void Echo(string text);
        ITemplate Template(string name, string version);
        IVM VM(string name);
        void Lab(string name, string author, string description);
        void LockAction(string action);
        void Property(string name, string value);
        string GetProperty(string name);
        void Action(string name, Action<string[], ISession> action);
    }
}