using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Pivotal.Extensions.Configuration.ConfigServer;

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
                .UseCloudFoundryHosting()
                .UseStartup<Startup>()
                .AddConfigServer(new LoggerFactory().AddConsole(LogLevel.Trace))
                .UseApplicationInsights()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("Pics")
                .Build();
    }
}
