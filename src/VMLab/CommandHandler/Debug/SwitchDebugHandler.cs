using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VMLab.Helper;

namespace VMLab.CommandHandler.Debug
{
    public class SwitchDebugHandler : BaseParamHandler
    {
        private readonly ISwitchParser _parser;
        private readonly IConsole _console;

        public SwitchDebugHandler(ISwitchParser parser, IConsole console, IUsage usage) : base(usage)
        {
            _parser = parser;
            _console = console;
        }

        public override string Group => "debug";
        public override string[] Handles => new[] {"switch", "s"};

        public override void OnHandle(string[] args)
        {
            var switches = args.Length != 0 ? _parser.Parse(args.Skip(1).ToArray()) : new Dictionary<string, string[]>();

            _console.Information(JsonConvert.SerializeObject(switches));
            _console.Pause("Press any key to continue...");

        }

        public override string UsageDescription => "Prints all the switches passed in out to the console for debuging.";

    }
}
