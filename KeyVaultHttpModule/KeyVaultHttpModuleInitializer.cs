using System.Configuration;
using System.Web;
using KeyVaultProvider;

namespace KeyVaultHttpModule
{
    public class KeyVaultHttpModuleInitializer : IHttpModule
    {
        private readonly KeyVaultConnectionStringFactory _connectionStringFactory;

        public KeyVaultHttpModuleInitializer()
        {
            _connectionStringFactory = new KeyVaultConnectionStringFactory();
        }

        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
            SetConnectionString();
        }

        private const string ConnectionStringName = "ConnectionStringName";
        private void SetConnectionString()
        {
      
        string connectionStringName = ConfigurationManager.AppSettings[ConnectionStringName];
            string newConnectionString = _connectionStringFactory.CreateConnectionString(connectionStringName);

            SetConnectionString(connectionStringName, newConnectionString);
        }

        private void SetConnectionString(string connectionStringName, string value)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
            connectionStringSetting.SetConnectionString(value);
        }

        public void Dispose() { }
    }
}
