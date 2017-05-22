using System.Collections.Generic;

namespace VMLab.CommandHandler
{
    public class SwitchParser : ISwitchParser
    {
        public IDictionary<string, string[]> Parse(string[] args)
        {
            var result = new Dictionary<string, string[]>();

            var currentOption = string.Empty;
            var values = new List<string>();

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (!string.IsNullOrEmpty(currentOption))
                        result.Add(currentOption, values.ToArray());

                    currentOption = arg.Substring(1);
                    values.Clear();
                }
                else
                {
                    values.Add(arg);
                }
            }

            result.Add(currentOption, values.ToArray());

            return result;
        }
    }
}
