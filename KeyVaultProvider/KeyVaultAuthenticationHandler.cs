using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace KeyVaultProvider
{

    //todo: split classe into separate classes per handler with same interface and inject parameters via constructor
    public static class KeyVaultAuthenticationHandler
    {
        //the method that will be provided to the KeyVaultClient
        public static async Task<string> GetTokenViaSecret(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority, TokenCache.DefaultShared);
            ClientCredential clientCred = new ClientCredential(ClientId, ClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            return GetAccessToken(result);
        }

        public static async Task<string> GetTokenViaCert(string authority, string resource, string scope)
        {
            ClientAssertionCertificate assertionCertificate = GetAuthenticationCertificate();

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, assertionCertificate);

            return GetAccessToken(result);
        }

        //this requires no key, just the service itself enabling it
        private static string GetAccessToken(AuthenticationResult result)
        {
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        private static string ClientId => ConfigurationManager.AppSettings["ClientId"];
        private static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];

        private static string ClientCertificateName => ConfigurationManager.AppSettings["ClientCertificate"];

        private static ClientAssertionCertificate GetAuthenticationCertificate()
        {
            X509Certificate2 clientCert = CertificateHelper.FindCertificate(ClientCertificateName);
            var authenticationCertificate = new ClientAssertionCertificate(ClientId, clientCert);

            return authenticationCertificate;
        }
    }
}
