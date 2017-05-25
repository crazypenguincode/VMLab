using System;
using System.Security.Cryptography;
using System.Text;

namespace VMLab.Contract.Helpers
{
    public class PasswordCryptoHelper : IPasswordCryptoHelper
    {
        public string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var buffer = Convert.FromBase64String(text);
            var results = ProtectedData.Unprotect(buffer, new byte[] { }, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(results);

        }

        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;


            var buffer = Encoding.UTF8.GetBytes(text);
            var results = ProtectedData.Protect(buffer, new byte[] { }, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(results);

        }
    }
}
