using System;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script.FluentInterface;

namespace VMLab.Script
{
    public class ScriptGlobal : IScriptGlobal
    {

        private readonly IConsole _console;
        private readonly Func<ITemplate> _templateFactory;
        private readonly Func<IVM> _vmFactory;
        private readonly IGraphManager _graphManager;

        public ScriptGlobal(IConsole console, Func<ITemplate> templateFactory, Func<IVM> vmFactory, IGraphManager graphManager)
        {
            _console = console;
            _templateFactory = templateFactory;
            _vmFactory = vmFactory;
            _graphManager = graphManager;
        }

        public void Echo(string text)
        {
            _console.Information(text);
        }

        public ITemplate Template(string name, string version)
        {
            var template = _templateFactory();
            template.Name = name;
            template.Version = version;
            return template;
        }

        public IVM VM(string name)
        {
            var vm = _vmFactory();
            vm.Name = name;

            return vm;
        }

        public void Lab(string name, string author, string description)
        {
            _graphManager.LabName = name;
            _graphManager.LabAuthor = author;
            _graphManager.LabDescription = description;
        }

        public void LockAction(string action)
        {
            _graphManager.AddLock(action);
        }

        public void Property(string name, string value)
        {
            //add property to graph
        }

        public string GetProperty(string name)
        {
            //get property from graph
            return null;
        }

        public void Action(string name, Action<string[], ISession> action)
        {
            //add action to graph.
        }
    }
}
