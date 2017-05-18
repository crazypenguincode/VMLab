using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Helper
{
    public class Config : IConfig
    {
        public string GetSetting(string setting, ConfigScope scope = ConfigScope.Merged)
        {
            //throw new NotImplementedException();
            return "VMwareWorkstation";
        }

        public string WriteSetting(string setting, string value, ConfigScope scope)
        {
            throw new NotImplementedException();
        }
    }
}
