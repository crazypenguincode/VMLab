using System;
using System.Text.RegularExpressions;

namespace VMLab.Contract.SemVer
{
    public class SemVer : IComparable<SemVer>
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string Tag { get; set; }

        public SemVer(string version) 
        {
            var match = Regex.Match(version, "^([0-9]{1,10})\\.([0-9]{1,10})\\.([0-9]{1,10})(|.)$");

            if(!match.Success)
                throw new ArgumentException(nameof(version));

            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            Patch = int.Parse(match.Groups[3].Value);

            if(match.Groups[4].Value.Length > 2)
                Tag = match.Groups[4].Value.Substring(1);
        }

        public int CompareTo(SemVer other)
        {
            if (other.Major > Major)
                return -1;
            if (other.Major < Major)
                return 1;
            if (other.Minor > Minor)
                return -1;
            if (other.Major < Minor)
                return 1;
            if (other.Patch > Patch)
                return -1;
            if (other.Patch < Patch)
                return 1;

            if (string.IsNullOrEmpty(Tag) && !string.IsNullOrEmpty(other.Tag))
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Tag) ? $"{Major}.{Minor}.{Patch}" : $"{Major}.{Minor}.{Patch}-{Tag}";
        }
    }
}
