using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KeyVaultHttpModule
{
    public class KeyVaultHttpModuleInitializer : IHttpModule
    {
        protected internal const int LoginFailedErrorNumber = 18456;
        //private readonly IConnectionStringFactory _connectionStringFactory;

        public KeyVaultHttpModuleInitializer()
        {
            //_connectionStringFactory = new KeyVaultConnectonStringFactory();
        }

        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
           // var dbTarget = LogManager.Configuration.AllTargets.OfType<DatabaseTarget>().FirstOrDefault();
            //bool hasDbTarget = dbTarget != null;
            //if (hasDbTarget)
            {
                //string connectionName = dbTarget.Name;
                //string logginConnectionString = _connectionStringFactory.CreateConnectionString("SchoolContext");
               // var layout = new SimpleLayout(logginConnectionString);
               // dbTarget.ConnectionString = layout;

                //LogManager.ReconfigExistingLoggers();
            }

            application.Error += ApplicationOnError;
        }

        private void ApplicationOnError(object sender, EventArgs e)
        {
            Exception lastException = HttpContext.Current.Server.GetLastError();

            if (lastException is SqlException lastSqlError && lastSqlError.Number == LoginFailedErrorNumber)
            {

            }
        }

        public void Dispose() { }
    }
}
