using System;
using VMLab.GraphModels;
using VMLab.Helper;

namespace VMLab.Script.FluentInterface
{
    public class TemplateFluentHandler : ITemplate
    {
        private readonly Template _template = new Template();
        private readonly IConfig _config;

        public TemplateFluentHandler(IGraphManager graphManager, IConfig config)
        {
            _config = config;
            graphManager.AddTemplate(_template);
        }

        public string Name
        {
            get => _template.Name;
            set => _template.Name = value;
        }

        public string Version
        {
            get => _template.Version;
            set => _template.Version = value;
        }

        public ITemplate Credential(string group, string username, string password)
        {
            _template.Credentials.Add(new Credential {Group = group, Username = username, Password = password});
            return this;
        }

        public ITemplate ISO(string path, string url)
        {
            _template.ISO = new ISO{ LocalPath =  path, URL =  url};
            return this;
        }

        public ITemplate GuestOS(string os, string arch)
        {
            switch (arch.ToLower())
            {
                case "x64":
                    _template.Arch = Arch.X64;
                    break;
                case "x86":
                    _template.Arch = Arch.X86;
                    break;
                default:
                    throw new ArgumentException("arch");
            }

            switch (os.ToLower())
            {
                case "nanoserver":
                    _template.GuestOS = GraphModels.GuestOS.Nano;
                    break;
                case "windows2016":
                    _template.GuestOS = GraphModels.GuestOS.Windows2016;
                    break;
                case "windows2016core":
                    _template.GuestOS = GraphModels.GuestOS.Windows2016Core;
                    break;
                case "windows2012r2":
                    _template.GuestOS = GraphModels.GuestOS.Windows2012R2;
                    break;
                case "windows2012r2core":
                    _template.GuestOS = GraphModels.GuestOS.Windows2012R2Core;
                    break;
                case "windows2012":
                    _template.GuestOS = GraphModels.GuestOS.Windows2012;
                    break;
                case "windows2012core":
                    _template.GuestOS = GraphModels.GuestOS.Windows2012Core;
                    break;
                case "windows2008r2":
                    _template.GuestOS = GraphModels.GuestOS.Windows2008R2;
                    break;
                case "windows2008r2core":
                    _template.GuestOS = GraphModels.GuestOS.Windows2008R2Core;
                    break;
                case "windows10":
                    _template.GuestOS = GraphModels.GuestOS.Windows10;
                    break;
                case "windows81":
                    _template.GuestOS = GraphModels.GuestOS.Windows81;
                    break;
                case "windows8":
                    _template.GuestOS = GraphModels.GuestOS.Windows8;
                    break;
                case "windows7":
                    _template.GuestOS = GraphModels.GuestOS.Windows7;
                    break;
                default:
                    throw new ArgumentException("os");
            }

            return this;
        }

        public ITemplate HardDisk(int size)
        {
            _template.HardDisks.Add(new HardDisk { Size = size });
            return this;
        }

        public ITemplate Network(string type, string name)
        {
            var net = new Network{Name = name};

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

            _template.Networks.Add(net);

            return this;

        }

        public ITemplate CPU(int cpus, int cores)
        {
            _template.CPUs = cpus;
            _template.CPUCores = cores;
            return this;
        }

        public ITemplate Memory(int size)
        {
            _template.Memory = size;
            return this;
        }

        public ITemplate FloppyImage(string path)
        {
            _template.FloppyImage = path;
            return this;
        }

        public ITemplate WithHypervisor(string hypervisor, Action<ITemplate> action)
        {
            if (string.Equals(_config.GetSetting("Hypervisor"), hypervisor, StringComparison.CurrentCultureIgnoreCase))
            {
                action(this);
            }

            return this;
        }

        public ITemplate OnProvision(Action<IVMControl> action)
        {
            _template.OnProvision = action;
            return this;
        }

        public ITemplate Healdess(bool headless)
        {
            _template.HeadLess = headless;
            return this;
        }
    }
}
