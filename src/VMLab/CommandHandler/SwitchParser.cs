using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.CommandHandler
{
    /// <summary>
    /// Parses a argument string and returns a dictionary containing all the switches and parameter values for them.
    /// </summary>
    public class SwitchParser : ISwitchParser
    {
        /// <summary>
        /// Parses a command string and extracts all commandline switches into a dictionary.
        /// </summary>
        /// <param name="args">Command line argument array.</param>
        /// <returns>Dictionary containing all the switches and their parsed values</returns>
        /// <exception cref="ArgumentException">Will throw if argument string doesn't start with a switch.</exception>
        public IDictionary<string, string[]> Parse(string[] args)
        {
            var result = new Dictionary<string, string[]>();

            if (args.Length == 0)
                return result;

            if(!args[0].StartsWith("-"))
                throw new ArgumentException(nameof(args));

            var currentOption = string.Empty;
            var values = new List<string>();

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if(currentOption != string.Empty)
                        result.Add(currentOption, values.ToArray());

                    currentOption = arg.Substring(arg.Substring(0,2) == "--" ? 2 : 1);

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

        /// <summary>
        /// Takes a dictionry create by Parse method and validates it against a ruleset.
        /// </summary>
        /// <param name="parsedResults">Prased switches from a command args string.</param>
        /// <param name="requirements">Requirement rules to validate switches against.</param>
        /// <returns></returns>
        public bool Validate(Dictionary<string, string[]> parsedResults, List<SwitchRequirementDefinition> requirements)
        {
            if(parsedResults.Keys.Any(p => requirements.All(r => r.Name.All(n  => n != p))))
                return false;

            if (parsedResults.Any(p => requirements.First(r => r.Name.Contains(p.Key)).Max < p.Value.Length))
                return false;

            if (parsedResults.Any(p => requirements.First(r => r.Name.Contains(p.Key)).Min > p.Value.Length))
                return false;

            return true;
        }
    }
}
