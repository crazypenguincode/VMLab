namespace VMLab.GraphModels
{
    public enum NetworkType
    {
        Bridged,
        NAT,
        Private
    }

    public class Network
    {
        public NetworkType Type { get; set; }
        public string Name { get; set; }

    }
}
