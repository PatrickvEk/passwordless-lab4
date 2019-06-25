namespace KeyVaultProvider
{
    public interface IConnectionStringFactory
    {
        string CreateConnectionString(string connectionStringName);
    }
}