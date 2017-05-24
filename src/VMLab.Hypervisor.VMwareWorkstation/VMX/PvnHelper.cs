using System;
using System.Collections.Generic;
using System.Text;
using SystemInterface.IO;
using Newtonsoft.Json;
using VMLab.Helper;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public class PVNHelper : IPVNHelper
    {
        private readonly IConfig _config;
        private readonly IFile _file;

        private const int PVNPartCount = 16;
        private const int PVNMiddlePart = 7;

        public PVNHelper(IConfig config, IFile file)
        {
            _config = config;
            _file = file;
        }

        public string GetPVN(string name)
        {
            var configFile = _config.GetSetting("GlobalSettingsDir", ConfigScope.System);

            var pvnList = new Dictionary<string, string>();

            if (_file.Exists(configFile))
            {
                pvnList = JsonConvert.DeserializeObject<Dictionary<string, string>>(_file.ReadAllText(configFile));
            }

            if (pvnList.ContainsKey(name))
                return pvnList[name];

            while (true)
            {
                var pvn = GeneratePVN();

                if (pvnList.ContainsValue(pvn)) continue;

                pvnList.Add(name, pvn);
                break;
            }

            _file.WriteAllText(configFile, JsonConvert.SerializeObject(pvnList));

            return pvnList[name];
        }

        private static string GeneratePVN()
        {
            var rand = new Random();
            var pvnstring = new StringBuilder();
            
            for (var i = 0; i < PVNPartCount; i++)
            {
                pvnstring.Append($"{rand.Next(0, 255):X2}");
                pvnstring.Append(i == PVNMiddlePart ? "-" : " ");
            }

            return pvnstring.ToString();
        }
    }
}
