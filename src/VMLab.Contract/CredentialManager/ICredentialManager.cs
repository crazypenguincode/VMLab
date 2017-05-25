using System.Collections.Generic;
using VMLab.GraphModels;

namespace VMLab.Contract.CredentialManager
{
    public interface ICredentialManager
    {
        void AddSecureCredential(Credential cred, VM vm);
        void AddGraphCredentail(Credential cred, VM vm);
        void AddGraphCredential(Credential cred, Template template);
        void LoadSecureCredentials(VM vm);
        Credential ResolveCredential(string group, VM vm);
        Credential ResolveCredential(string group, Template template);
        IEnumerable<Credential> AllCredentials(VM vm);
        IEnumerable<Credential> AllCredentials(Template template);
        void RemoveSecureCredential(string group, VM vm);
        void ClearAllSecureCredentail();
    }
}
