using Basket.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Microsoft.eShopOnContainers.Services.Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
                .ConfigureAppConfiguration(c => c.AddCloudFoundry())
                .UseCloudFoundryHosting()
                .AddCloudFoundry()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                //.UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("Pics")
                // .UseFailing(options =>
                // {
                //     options.ConfigPath = "/Failing";
                // })
              //  .UseHealthChecks("/hc")
                // .ConfigureAppConfiguration((builderContext, config) =>
                // {
                //     var builtConfig = config.Build();

                //     var configurationBuilder = new ConfigurationBuilder();

                //     if (Convert.ToBoolean(builtConfig["UseVault"]))
                //     {
                //         configurationBuilder.AddAzureKeyVault(
                //             $"https://{builtConfig["Vault:Name"]}.vault.azure.net/",
                //             builtConfig["Vault:ClientId"],
                //             builtConfig["Vault:ClientSecret"]);
                //     }

                //     configurationBuilder
                //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //         .AddEnvironmentVariables();

                //     config.AddConfiguration(configurationBuilder.Build());
                // })
                // .ConfigureLogging((hostingContext, builder) =>
                // {
                //     builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //     builder.AddConsole();
                //     builder.AddDebug();
                // })
                .Build();
    }
}
