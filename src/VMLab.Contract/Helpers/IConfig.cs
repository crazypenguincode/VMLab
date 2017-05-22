namespace VMLab.Helper
{
    public enum ConfigScope
    {
        System,
        User,
        Lab,
        Merged
    }
    public interface IConfig
    {
        string GetSetting(string setting, ConfigScope scope = ConfigScope.Merged);
        void WriteSetting(string setting, string value, ConfigScope scope);
        string Dump(ConfigScope scope);
    }
}
