using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using KeyVaultProvider;

namespace KeyVaultConnectionProvider
{
    public class KeyVaultConnectionFactory : IDbConnectionFactory
    {
        public const string ConnectionStringNameSuffix = "_KeyVault";

        private readonly IDbConnectionFactory _baseConnectionFactory;
        private readonly ICachedConnectionStringFactory _connectionStringFactory;
        private readonly IDbExecutionStrategy _retrySqlStrategy;

        public KeyVaultConnectionFactory()
        {
            // we would use some decoupling here
            var connectionStringFactory = new KeyVaultConnectionStringFactory()
            {
                ConnectionStringNameSuffix = ConnectionStringNameSuffix
            };

            _connectionStringFactory = new CachedConnectionStringFactory(connectionStringFactory);

            _retrySqlStrategy = new RetrySqlInvalidPasswordStrategy(FailedConnectionAttempt);
            _baseConnectionFactory = new SqlConnectionFactory();
        }

        public void FailedConnectionAttempt(Exception lastException)
        {
            _connectionStringFactory.IsDirty = true;
            Debug.WriteLine("FAILED ATTEMPT");
        }

        public DbConnection CreateConnection(string connectionStringName)
        {
            DbConnection connection = _retrySqlStrategy.Execute(() => CreateOpenConnection(connectionStringName));
            return connection;
        }

        private DbConnection CreateOpenConnection(string connectionStringName)
        {
            string connectionString = _connectionStringFactory.CreateConnectionString(connectionStringName);
            DbConnection sqlConnection = _baseConnectionFactory.CreateConnection(connectionString);

            // open connection to verify credentials
            sqlConnection.Open();

            RunTestQuerySettingBased(sqlConnection);

            return sqlConnection;
        }

        private static void RunTestQuerySettingBased(DbConnection sqlConnection)
        {
            bool runTestQuery = GetTestQuerySetting();

            if (runTestQuery)
            {
                RunTestQuery(sqlConnection);
            }
        }

        private static bool GetTestQuerySetting()
        {
            bool runTestQuery = false;
            string runTestQuerySetting = ConfigurationManager.AppSettings["RunTestQuery"];
            if (runTestQuerySetting != null)
            {
                bool.TryParse(runTestQuerySetting, out runTestQuery);
            }

            return runTestQuery;
        }

        private static void RunTestQuery(DbConnection sqlConnection)
        {
            using (DbCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = "select @@version";

                command.ExecuteNonQuery();
            }
        }
    }
}
