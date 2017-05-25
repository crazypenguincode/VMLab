using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VMLab.Contract.Helpers;
using VMLab.GraphModels;
using VMLab.Helper;

namespace VMLab.Contract.CredentialManager
{
    public class CredentialManagerSingleton : ICredentialManager
    {
        private readonly Dictionary<string, List<Credential>> _credentials = new Dictionary<string, List<Credential>>();
        private readonly IConfig _config;
        private readonly IPasswordCryptoHelper _passwordCryptoHelper;

        public CredentialManagerSingleton(IConfig config, IPasswordCryptoHelper passwordCryptoHelper)
        {
            _config = config;
            _passwordCryptoHelper = passwordCryptoHelper;
        }


        public void AddSecureCredential(Credential cred, VM vm)
        {
            cred.Secure = true;
            if(!_credentials.ContainsKey(vm.Name))
                _credentials.Add(vm.Name, new List<Credential>());

            _credentials[vm.Name].RemoveAll(c => c.Group == cred.Group);
            _credentials[vm.Name].Add(cred);
            

            var text = _passwordCryptoHelper.Decrypt(_config.GetSetting($"Passwords.{vm.Name}", ConfigScope.Lab));
            var keystore = string.IsNullOrEmpty(text) ? new List<Credential>() : new List<Credential>(JsonConvert.DeserializeObject<Credential[]>(text));

            keystore.RemoveAll(c => c.Group == cred.Group);
            keystore.Add(cred);

            text = _passwordCryptoHelper.Encrypt(JsonConvert.SerializeObject(keystore.ToArray()));
            _config.WriteSetting($"Passwords.{vm.Name}", text, ConfigScope.Lab);
        }

        public void AddGraphCredentail(Credential cred, VM vm)
        {
            if (!_credentials.ContainsKey(vm.Name))
                _credentials.Add(vm.Name, new List<Credential>());

            _credentials[vm.Name].RemoveAll(c => c.Group == cred.Group);
            _credentials[vm.Name].Add(cred);
        }

        public void AddGraphCredential(Credential cred, Template template)
        {
            if (!_credentials.ContainsKey(template.Name))
                _credentials.Add(template.Name, new List<Credential>());

            _credentials[template.Name].RemoveAll(c => c.Group == cred.Group);
            _credentials[template.Name].Add(cred);
        }

        public void LoadSecureCredentials(VM vm)
        {
            var text = _passwordCryptoHelper.Decrypt(_config.GetSetting($"Passwords.{vm.Name}", ConfigScope.Lab));
            var keystore = string.IsNullOrEmpty(text) ? new List<Credential>() : new List<Credential>(JsonConvert.DeserializeObject<Credential[]>(text));

            foreach (var c in keystore)
                AddGraphCredentail(c, vm);
        }

        public Credential ResolveCredential(string group, VM vm)
        {
            return !_credentials.ContainsKey(vm.Name) ? null 
                : _credentials[vm.Name].FirstOrDefault(c => c.Group == group);
        }

        public Credential ResolveCredential(string group, Template template)
        {
            return !_credentials.ContainsKey(template.Name) ? null
                : _credentials[template.Name].FirstOrDefault(c => c.Group == group);
        }

        public IEnumerable<Credential> AllCredentials(VM vm)
        {
            return !_credentials.ContainsKey(vm.Name) ? null : _credentials[vm.Name];
        }

        public IEnumerable<Credential> AllCredentials(Template template)
        {
            return !_credentials.ContainsKey(template.Name) ? null : _credentials[template.Name];
        }

        public void RemoveSecureCredential(string group, VM vm)
        {
            var text = _passwordCryptoHelper.Decrypt(_config.GetSetting($"Passwords.{vm.Name}", ConfigScope.Lab));
            var keystore = string.IsNullOrEmpty(text) ? new List<Credential>() : new List<Credential>(JsonConvert.DeserializeObject<Credential[]>(text));

            keystore.RemoveAll(c => c.Group == group);

            text = _passwordCryptoHelper.Encrypt(JsonConvert.SerializeObject(keystore.ToArray()));
            _config.WriteSetting($"Passwords.{vm.Name}", text, ConfigScope.Lab);
        }

        public void ClearAllSecureCredentail()
        {
            _config.Clean("^Passwords\\..*", ConfigScope.Lab);
        }
    }
}
