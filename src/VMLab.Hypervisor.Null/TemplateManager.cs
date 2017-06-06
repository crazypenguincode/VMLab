using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMLab.Contract;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.Null
{
    public class TemplateManager : ITemplateManager
    {
        public bool CanBuild(Template template)
        {
            throw new NotImplementedException();
        }

        public void Build(Template template, string templateFolder)
        {
            throw new NotImplementedException();
        }

        public void BuildVMFromTemplate(VM vm)
        {
            throw new NotImplementedException();
        }

        public void ImportTemplate(string path)
        {
            throw new NotImplementedException();
        }

        public void RemoveTemplate(string name)
        {
            throw new NotImplementedException();
        }
    }
}
