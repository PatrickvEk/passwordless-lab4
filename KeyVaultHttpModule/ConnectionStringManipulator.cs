using System.Configuration;
using System.Reflection;

namespace KeyVaultHttpModule
{
    public static class ConnectionStringManipulator
    {
        private static readonly FieldInfo ConfigurationElementReadonlyField = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);


        public static void SetConnectionString(this ConnectionStringSettings connectionStringSetting, string value)
        {
            // this is a bit dirty
            SetReadOnly(connectionStringSetting, false);
            connectionStringSetting.ConnectionString = value;
            SetReadOnly(connectionStringSetting, true);
        }

        private static void SetReadOnly(ConnectionStringSettings connectionStringSetting, bool flag)
        {
            ConfigurationElementReadonlyField.SetValue(connectionStringSetting, flag);
        }
    }
}
