using System.Collections.Generic;

namespace VMLab.GraphModels
{
    public class GraphManagerSingleton : IGraphManager
    {

        private readonly List<Template> _templates;
        private readonly List<VM> _vms;
        private readonly List<string> _locks;
        private readonly List<Action> _actions;

        public GraphManagerSingleton()
        {
            _templates = new List<Template>();
            _vms = new List<VM>();
            _locks = new List<string>();
            _actions = new List<Action>();
        }

        public IEnumerable<Template> Templates => _templates;
        public IEnumerable<VM> VMs => _vms;
        public string LabName { get; set; }
        public string LabAuthor { get; set; }
        public string LabDescription { get; set; }

        public IEnumerable<string> Locks => _locks;
        public IEnumerable<Action> Actions => _actions;

        public void AddTemplate(Template template)
        {
            _templates.Add(template);
        }

        public void AddVM(VM vm)
        {
            _vms.Add(vm);
        }

        public void AddLock(string name)
        {
            _locks.Add(name);
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }
    }
}
