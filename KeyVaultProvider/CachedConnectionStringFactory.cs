using System.Diagnostics;
using System.Threading;

namespace KeyVaultProvider
{
    public class CachedConnectionStringFactory : ICachedConnectionStringFactory
    {
        private readonly IConnectionStringFactory _baseConnectionStringFactory;
        private string _cachedConnectionString;

        private bool _isInitialized = false;
        private object _dataLock = new object();

        public CachedConnectionStringFactory(IConnectionStringFactory baseConnectionStringFactory)
        {
            _baseConnectionStringFactory = baseConnectionStringFactory;
        }

        public bool IsDirty
        {
            get => _isInitialized;
            set => _isInitialized = !value;
        }

        public string CreateConnectionString(string connectionStringName)
        {
            // more info https://docs.microsoft.com/en-us/dotnet/api/system.threading.lazyinitializer
            string initializedConnectionString = LazyInitializer.EnsureInitialized(ref _cachedConnectionString, ref _isInitialized, ref _dataLock, () => GetNewConnectionString(connectionStringName));

            Debug.WriteLine($"CREATE CONNECTION {initializedConnectionString}");
            return initializedConnectionString; ;
        }

        private string GetNewConnectionString(string connectionStringName)
        {
            string newConnectionString = _baseConnectionStringFactory.CreateConnectionString(connectionStringName);

            Debug.WriteLine($"KEYVAULT LOOKUP{newConnectionString}");
            return newConnectionString;
        }
    }
}