using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    public class HomeController : Controller
    {
        IConfigurationRoot Config { get; set; }

        public HomeController(IConfigurationRoot config)
        {
            Config = config;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }

        public IActionResult RefreshConfiguration()
        {
            if (Config != null)
            {
                Config.Reload();
            }

            return View();
        }
    }
}
