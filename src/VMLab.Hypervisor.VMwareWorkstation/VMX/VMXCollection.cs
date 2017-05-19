using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SystemInterface.IO;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public class VMXCollection : IVMXCollection
    {
        private readonly List<VMXItem> _vmxdata = new List<VMXItem>();
        private readonly IFile _file;

        public VMXCollection(IFile file)
        {
            _file = file;
        }

        public string ReadValue(string name)
        {
            var item = _vmxdata.FirstOrDefault(i => i.Name == name);

            return item?.Value;
        }

        public void WriteValue(string name, string value)
        {
            var item = _vmxdata.FirstOrDefault(i => i.Name == name);

            if(item == default(VMXItem))
                item = new VMXItem();

            _vmxdata.Add(item);

            item.Name = name;
            item.Value = value;
        }

        public void ReadFromFile(string path)
        {
            _vmxdata.Clear();

            foreach (var line in _file.ReadAllLines(path))
            {
                var match = Regex.Match(line, "(.+) = \"(.+)\"");

                if (match.Success)
                {
                    WriteValue(match.Groups[1].Value, match.Groups[2].Value);
                }
            }
        }

        public void WriteToFile(string path)
        {
            var text = (from l in _vmxdata
                select $"{l.Name} = \"{l.Value}\"").ToArray();

            _file.WriteAllLines(path, text, Encoding.UTF8);
        }
    }
}
