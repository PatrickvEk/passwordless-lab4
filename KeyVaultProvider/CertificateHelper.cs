using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace KeyVaultProvider
{
    public static class CertificateHelper
    {
        public static X509Certificate2 FindCertificate(string findValue)
        {
            X509Certificate2 currentUser = FindCertificate(findValue, StoreName.My, StoreLocation.CurrentUser);
            if (currentUser != null)
            {
                return currentUser;
            }

            // be aware, getting certificates from localmachine usually requires running in elevated mode (administrator)
            X509Certificate2 localMachine = FindCertificate(findValue, StoreName.My, StoreLocation.LocalMachine);
            return localMachine;
        }

        public static X509Certificate2 FindCertificate(string findValue, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection collection = store.Certificates.Find(X509FindType.FindBySubjectName, findValue, false);

                return collection.OfType<X509Certificate2>().FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}