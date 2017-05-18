using VMLab.CommandHandler;
using VMLab.IOC;

namespace VMLab
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Bootstrap().Start<ICommandHandler>().Parse(args);
        }
    }
}
