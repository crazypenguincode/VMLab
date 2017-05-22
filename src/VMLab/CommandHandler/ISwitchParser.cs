using System.Collections.Generic;

namespace VMLab.CommandHandler
{
    public interface ISwitchParser
    {
        IDictionary<string, string[]> Parse(string[] args);
    }
}
