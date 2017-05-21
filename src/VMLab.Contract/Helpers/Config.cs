using System;
using System.Collections.Generic;
using SystemInterface;
using SystemInterface.IO;
using Newtonsoft.Json;

namespace VMLab.Helper
{
    public class Config : IConfig
    {
        private readonly IEnvironment _environment;
        private readonly IDirectory _directory;
        private readonly IFile _file;

        public Config(IEnvironment environment, IDirectory directory, IFile file)
        {
            _environment = environment;
            _directory = directory;
            _file = file;
        }

        public string GetSetting(string setting, ConfigScope scope = ConfigScope.Merged)
        {

            if (scope != ConfigScope.Merged)
            {
                var store = GetSettings(scope);
                return store.ContainsKey(setting) ? store[setting] : null;
            }

            var system = GetSettings(ConfigScope.System);
            var user = GetSettings(ConfigScope.User);
            var lab = GetSettings(ConfigScope.Lab);

            if (lab.ContainsKey(setting))
                return lab[setting];

            if (user.ContainsKey(setting))
                return user[setting];

            return system.ContainsKey(setting) ? system[setting] : null;
        }

        public void WriteSetting(string setting, string value, ConfigScope scope)
        {
            if(scope == ConfigScope.Merged)
                throw new ArgumentException("scope");

            var settings = GetSettings(scope);

            if (settings.ContainsKey(setting))
                settings[setting] = value;
            else
                settings.Add(setting, value);

            WriteSettings(settings, scope);

        }

        private Dictionary<string, string> GetSettings(ConfigScope scope)
        {
            string path;

            switch (scope)
            {
                case ConfigScope.System:
                    path = _environment.ExpandEnvironmentVariables("%ProgramData%\\VMLab");
                    break;
                case ConfigScope.User:
                    path = _environment.ExpandEnvironmentVariables("%LOCALAPPDATA%\\VMLab");
                    break;
                case ConfigScope.Lab:
                    path = $"{_environment.CurrentDirectory}\\_vmlab";
                    break;
                case ConfigScope.Merged:
                    throw new ArgumentException("scope");
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
            

            if (!_directory.Exists(path))
                _directory.CreateDirectory(path);

            if (_file.Exists($"{path}\\settings.json"))
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    _file.ReadAllText($"{path}\\settings.json"));

            var result = new Dictionary<string, string>();

            if (scope != ConfigScope.System) return result;

            result.Add("Hypervisor", "VMwareWorkstation");
            result.Add("GlobalSettingsDir", _environment.ExpandEnvironmentVariables("%ProgramData%\\VMLab"));
            result.Add("UserSettingsDir", _environment.ExpandEnvironmentVariables("%LOCALAPPDATA%\\VMLab"));
            result.Add("TemplateDir", _environment.ExpandEnvironmentVariables("%ProgramData%\\VMLab\\Templates"));
            result.Add("TempDir", _environment.ExpandEnvironmentVariables("%Temp%\\VMLab"));

            return result;
        }

        private void WriteSettings(Dictionary<string, string> settings, ConfigScope scope)
        {
            string path;

            switch (scope)
            {
                case ConfigScope.System:
                    path = _environment.ExpandEnvironmentVariables("%ProgramData%\\VMLab");
                    break;
                case ConfigScope.User:
                    path = _environment.ExpandEnvironmentVariables("%LOCALAPPDATA%\\VMLab");
                    break;
                case ConfigScope.Lab:
                    path = $"{_environment.CurrentDirectory}\\_vmlab";
                    break;
                case ConfigScope.Merged:
                    throw new ArgumentException("scope");
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }

            if (!_directory.Exists(path))
                _directory.CreateDirectory(path);

            _file.WriteAllText($"{path}\\settings.json", JsonConvert.SerializeObject(settings));
        }
    }
}
