namespace VMLab.Contract.Helpers
{
    public interface IPasswordCryptoHelper
    {
        string Decrypt(string text);
        string Encrypt(string text);
    }
}
