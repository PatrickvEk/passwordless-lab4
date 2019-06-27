using System;
using System.Configuration;
using System.Data.SqlClient;

namespace KeyVaultProvider
{
    public class KeyVaultConnectionStringFactory : IConnectionStringFactory
    {
        private readonly string _secretUri;
        public string ConnectionStringNameSuffix { get; set; } = string.Empty;

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
                    throw new InvalidOperationException("SecretUri must be set via config (web.config/app.config)");
                }

                return secretUri;
            }
        }

        public string CreateConnectionString(string connectionStringName)
        {
            string connectionStringWithCreds = ConnectionStringWithCreds(connectionStringName);

            return connectionStringWithCreds;
        }

        private string ConnectionStringWithCreds(string connectionStringName)
        {
            string settingName = connectionStringName + ConnectionStringNameSuffix;

            ConnectionStringSettings connectionStringFromSettings = ConfigurationManager.ConnectionStrings[settingName];

            bool connectionStringFound = connectionStringFromSettings != null;
            if (!connectionStringFound)
            {
                throw new InvalidOperationException($"ConnectionString '{settingName}' in settings not found.");
            }

            string connectionStringWithCreds = AddCredentialsToConnectionString(connectionStringFromSettings.ConnectionString);
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