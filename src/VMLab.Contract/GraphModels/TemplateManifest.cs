using VMLab.GraphModels;

namespace VMLab.Contract.GraphModels
{
    public class TemplateManifest
    {
        public string Name { get; set; }
        public string Hypervisor { get; set; }
        public GuestOS OS { get; set; }
        public Arch Arch { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
    }
}
