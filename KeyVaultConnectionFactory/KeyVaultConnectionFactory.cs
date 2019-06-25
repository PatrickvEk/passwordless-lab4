using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using KeyVaultProvider;

namespace KeyVaultConnectionProvider
{
    public class KeyVaultConnectionFactory : IDbConnectionFactory
    {
        public const string ConnectionStringNameSuffix = "_KeyVault";

        private readonly IConnectionStringFactory _connectionStringFactory;

        public KeyVaultConnectionFactory()
        {
            _connectionStringFactory = new KeyVaultConnectionStringFactory()
            {
                ConnectionStringNameSuffix = ConnectionStringNameSuffix
            };
        }

        public DbConnection CreateConnection(string connectionStringName)
        {
            var builderConnectionString = _connectionStringFactory.CreateConnectionString(connectionStringName);
            SqlConnection sqlConnection = new SqlConnection(builderConnectionString);

            return sqlConnection;
        }
    }
}
