using System;
using System.Configuration;
using System.Data.SqlClient;

namespace KeyVaultProvider
{
    public class KeyVaultConnectionStringFactory : IConnectionStringFactory
    {
        private readonly string _secretUri;
        public string ConnectionStringNameSuffix { get; set; } = String.Empty;

        public KeyVaultConnectionStringFactory()
        : this(SecretUriViaSettings)
        {
        }

        public KeyVaultConnectionStringFactory(string secretUri)
        {
            _secretUri = secretUri;
        }

        public static string SecretUriViaSettings
        {
            get
            {
                string secretUri = ConfigurationManager.AppSettings["SecretUri"];

                if (secretUri == null)
                {
                    throw new ArgumentException("SecretUri", "SecretUri must be set via config (web.config/app.config)");
                }

                return secretUri;
            }
        }

        public string CreateConnectionString(string connectionStringName)
        {
            string settingName = connectionStringName + ConnectionStringNameSuffix;

            string connectionStringFromSettings = ConfigurationManager.ConnectionStrings[settingName].ToString();

            bool connectionStringFound = connectionStringFromSettings != null;
            if (!connectionStringFound)
            {
                throw new InvalidOperationException($"AppSetting '{settingName}' not found.");
            }

            string connectionStringWithCreds = AddCredentialsToConnectionString(connectionStringFromSettings);

            return connectionStringWithCreds;
        }

        private string AddCredentialsToConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            builder.Password = KeyVaultService.GetPassword(_secretUri);
            string builderConnectionString = builder.ConnectionString;
            return builderConnectionString;
        }
    }
}