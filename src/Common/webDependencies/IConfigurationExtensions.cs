using System;
using System.Collections.Generic;
using  Steeltoe.CloudFoundry.Connector.SqlServer;
using System.Text;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;

namespace webDependencies
{
    public static class IConfigurationExtensions
    {
        public static string GetConnection(this IConfiguration config, string serviceName = null)
        {
            return new SqlServerProviderConnectorFactory(!string.IsNullOrEmpty(serviceName) ? config.GetRequiredServiceInfo<SqlServerServiceInfo>(serviceName) : config.GetSingletonServiceInfo<SqlServerServiceInfo>(), new SqlServerProviderConnectorOptions(config), (Type)null).CreateConnectionString();
        }
    }
}
