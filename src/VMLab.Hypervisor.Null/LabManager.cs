using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;

namespace VMLab.Hypervisor.Null
{
    public class LabManager : ILabManager
    {
        public void ExportLab(string path)
        {
            throw new NotImplementedException();
        }

        public void ImportLab(string path)
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }

        public void Init(string templateName)
        {
            throw new NotImplementedException();
        }
    }
}
