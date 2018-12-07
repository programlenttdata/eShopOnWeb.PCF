using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pivotal.Extensions.Configuration.ConfigServer;
using System;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<CatalogContext>((context,services)=>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<CatalogSettings>>();
                    var logger = services.GetService<ILogger<CatalogContextSeed>>();

                    new CatalogContextSeed()
                    .SeedAsync(context,env,settings,logger)
                    .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_,__)=>{})
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
                //.UseKestrel()
                //.ConfigureAppConfiguration(c => c.AddCloudFoundry())
                .UseCloudFoundryHosting()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                //.AddCloudFoundry()
                .AddConfigServer()
                .UseApplicationInsights()
                .UseWebRoot("Pics")           
                .Build();    
    }
}

/*
  "rabbitmq": {
    "client": {
      "uri": "amqp://guest:guest@localhost:5672/"
    }
  },
*/