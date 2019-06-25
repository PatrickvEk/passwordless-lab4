using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeyVaultConnectionProvider;
using KeyVaultHttpModule;

namespace ContosoUniversity
{
    public class DeployKeyVaultHttpModule
    {
        public Type KeyVaultHttpModuleDependency = typeof(KeyVaultHttpModuleInitializer);
        public Type KeyVaultConnectionProviderDependency = typeof(KeyVaultConnectionFactory);
    }
}