using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    public class Program
    {
        private const string SQLSERVER_DB = @"
            {
                'SqlServer': [
                  
                    {
                        'name': 'SqlServer',
                        'credentials': {
                            'ConnectionString': 'server=eshop-ordering-rds.czvirbruk2y4.us-east-1.rds.amazonaws.com;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=eshoporderingapi;Password=Pass@word',
                            'uid': 'eshoporderingapi',
                            'uri': 'jdbc:sqlserver://eshop-ordering-rds.czvirbruk2y4.us-east-1.rds.amazonaws.com:1433;databaseName=Microsoft.eShopOnContainers.Services.OrderingDb;',
                            'db': 'Microsoft.eShopOnContainers.Services.OrderingDb',
                            'pw': '9XF2Ljg$GjjR'
                        },
                        'label': 'sqlserver',
                        'tags': [
                            'sqlserver'
                        ]
                    }
                ]
            }";
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", SQLSERVER_DB);
            BuildWebHost(args)
                .MigrateDbContext<OrderingContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<OrderingSettings>>();
                    var logger = services.GetService<ILogger<OrderingContextSeed>>();

                    new OrderingContextSeed()
                        .SeedAsync(context, env, settings, logger)
                        .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_,__)=>{})
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)                
                .UseStartup<Startup>()
                //.UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var builtConfig = config.Build();

                    var configurationBuilder = new ConfigurationBuilder();
                    
                    if (Convert.ToBoolean(builtConfig["UseVault"]))
                    {
                        configurationBuilder.AddAzureKeyVault(
                            $"https://{builtConfig["Vault:Name"]}.vault.azure.net/",
                            builtConfig["Vault:ClientId"],
                            builtConfig["Vault:ClientSecret"]);
                    }

                    configurationBuilder.AddEnvironmentVariables();

                    config.AddConfiguration(configurationBuilder.Build());
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .UseApplicationInsights()
                .Build();
    }
}
