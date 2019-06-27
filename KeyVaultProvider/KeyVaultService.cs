using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace KeyVaultProvider
{
    public class KeyVaultService
    {
        public static string GetPassword(string secretUri)
        {
            try
            {
                string plainPassword = Task.Run(() => GetPasswordAsync(secretUri)).GetAwaiter().GetResult(); //run sync
                return plainPassword;
            }
            catch (KeyVaultErrorException)
            {
                // does technically nothing, just helping the meetup-user

                if (Debugger.IsAttached)
                {
                    // if you reach here, your secretUri is probably wrong. Especially if it is a 'NotFound' error.

                    Debugger.Break();
                }

                throw;
            }
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