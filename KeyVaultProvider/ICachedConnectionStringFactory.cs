namespace KeyVaultProvider
{
    public interface ICachedConnectionStringFactory : IConnectionStringFactory
    {
        bool IsDirty { get; set; }
    }
}