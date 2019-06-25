using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace KeyVaultProvider
{
    public class KeyVaultService
    {
        public static string GetPassword(string secretUri)
        {
            string plainPassword = Task.Run(() => GetPasswordAsync(secretUri)).GetAwaiter().GetResult(); //run sync
            return plainPassword;
        }

        public static async Task<string> GetPasswordAsync(string secretUri)
        {
            var keyVaultClient = new KeyVaultClient(KeyVaultAuthenticationHandler.GetTokenViaCert);
            SecretBundle secretBundle = await keyVaultClient.GetSecretAsync(secretUri);
            string plainPassword = secretBundle.Value;

            return plainPassword;
        }
    }
}