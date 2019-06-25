using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
