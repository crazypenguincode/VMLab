using System;
using System.Collections.Generic;
using VMLab.GraphModels;
using VMLab.Helper;
using Action = System.Action;

namespace VMLab.Script.FluentInterface
{
    public class VMFluentHandler : IVM
    {
        private readonly IConfig _config;
        private readonly VM _vm;

        public VMFluentHandler(IConfig config, IGraphManager graphManager)
        {
            _config = config;

            _vm = new VM();
            graphManager.AddVM(_vm);
        }

        public string Name
        {
            get => _vm.Name;
            set => _vm.Name = value;
        }

        public IVM Template(string name, string version = "latest")
        {
            _vm.Template = name;
            _vm.Version = version;
            return this;
        }

        public IVM Credential(string group, string username, string password)
        {
            _vm.Credentials.Add(new Credential{Group = group, Username = username, Password =  password});
            return this;
        }

        public IVM Network(string type, string name = "")
        {
            var net = new Network { Name = name };

            switch (type.ToLower())
            {
                case "bridged":
                    net.Type = NetworkType.Bridged;
                    break;
                case "private":
                    net.Type = NetworkType.Private;
                    break;
                case "nat":
                    net.Type = NetworkType.NAT;
                    break;
                default:
                    throw new ArgumentException("type");
            }

            _vm.Networks.Add(net);

            return this;
        }

        public IVM CPU(int cpus, int cores)
        {
            _vm.CPUs = cpus;
            _vm.CPUCores = cores;

            return this;
        }

        public IVM Memory(int size)
        {
            _vm.Memeory = size;
            return this;
        }

        public IVM ShareFolder(string hostpath, string guestpath)
        {
            _vm.SharedFolders.Add(new SharedFolder{HostPath = hostpath, GuestPath = guestpath});
            return this;
        }

        public IVM WithHypervisor(string hypervisor, Action<IVM> action)
        {
            if (string.Equals(_config.GetSetting("Hypervisor"), hypervisor, StringComparison.CurrentCultureIgnoreCase))
            {
                action(this);
            }

            return this;
        }

        public IVM OnProvision(Action<IVMControl> action)
        {
            _vm.OnProvision = action;
            return this;
        }

        public IVM OnDestroy(Action<IVMControl> action)
        {
            _vm.OnDestroy = action;
            return this;
        }

        public IVM AfterDestroy(Action action)
        {
            _vm.AfterDestroy = action;
            return this;
        }

        public IVM Property(string name, string value)
        {
            if (_vm.Properties.ContainsKey(name))
                _vm.Properties[name] = value;
            else
                _vm.Properties.Add(name, value);

            return this;
        }

        public IVM NestedVirtualization()
        {
            _vm.NestedVirtualization = true;
            return this;
        }
    }
}
