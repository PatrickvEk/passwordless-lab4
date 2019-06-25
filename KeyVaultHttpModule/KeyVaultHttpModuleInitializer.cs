using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using KeyVaultProvider;

namespace KeyVaultHttpModule
{
    public class KeyVaultHttpModuleInitializer : IHttpModule
    {
        private const int LoginFailedErrorNumber = 18456;
        private const string ConnectionStringName = "ConnectionStringName";

        private readonly KeyVaultConnectionStringFactory _connectionStringFactory;

        public KeyVaultHttpModuleInitializer()
        {
            _connectionStringFactory = new KeyVaultConnectionStringFactory();
        }

        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
            //todo: make setting
            SetConnectionString();

            // for demo-purposes only
            application.Error += ApplicationOnError;
        }

        private void SetConnectionString()
        {
            string connectionStringName = ConfigurationManager.AppSettings[ConnectionStringName];
            string value = _connectionStringFactory.CreateConnectionString(connectionStringName);

            SetConnectionString(connectionStringName, value);
        }

        private void SetConnectionString(string connectionStringName, string value)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
            connectionStringSetting.SetConnectionString(value);
        }

        private void ApplicationOnError(object sender, EventArgs e)
        {
            Exception lastException = HttpContext.Current.Server.GetLastError();

            if (lastException is SqlException lastSqlError && lastSqlError.Number == LoginFailedErrorNumber)
            {
                try
                {
                    SetConnectionString();
                }
                catch (Exception exception)
                {
                    // ignore
                    Debug.WriteLine(exception);
                }
            }
        }

        public void Dispose() { }
    }
}
