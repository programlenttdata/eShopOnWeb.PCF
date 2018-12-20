using Microsoft.AspNetCore.Builder;
using Microsoft.eShopOnContainers.Services.Ordering.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ordering.FunctionalTests
{
    public class OrderingTestsStartup : Startup
    {
        public OrderingTestsStartup(ILogger<Startup> logger, IConfiguration env) : base(logger,env)
        {
            _logger = logger;
        }
        private readonly ILogger<Startup> _logger;

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
