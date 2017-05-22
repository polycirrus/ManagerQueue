using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;

using CredentialManagement;

namespace BSUIR.ManagerQueue.Client.Models
{
    public static class CredentialManager
    {
        public static Tuple<string, string> GetCredential()
        {
            var credential = new Credential();
            credential.Target = GetProductName();
            credential.Load();

            if (string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password))
            {
                credential.Dispose();
                return null;
            }

            var result = new Tuple<string, string>(credential.Username, credential.Password);
            credential.Dispose();
            return result;
        }

        public static void SaveCredential(string userName, SecureString password)
        {
            var credential = new Credential(userName);
            credential.SecurePassword = password;
            credential.Target = GetProductName();
            credential.PersistanceType = PersistanceType.LocalComputer;

            credential.Save();

            credential.Dispose();
        }

        private static string GetProductName()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            return versionInfo.ProductName;
        }
    }
}
