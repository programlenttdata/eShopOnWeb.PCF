using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;
using Polly.CircuitBreaker;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.eShopWeb.Infrastructure.Identity;
//using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Interfaces;

namespace Microsoft.eShopWeb.Web.Controllers
{
    [Route("basket/[action]")]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogSvc;
        private readonly Microsoft.eShopWeb.ApplicationCore.Interfaces.IIdentityParser<ApplicationUser> _appUserParser;

        public BasketController(IBasketService basketSvc, ICatalogService catalogSvc, ApplicationCore.Interfaces.IIdentityParser<ApplicationUser> appUserParser)
        {
            _basketSvc = basketSvc;
            _catalogSvc = catalogSvc;
            _appUserParser = appUserParser;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var vm = await _basketSvc.GetBasket(user.Name);

                return View(vm);
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> mybasket)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var vm = await _basketSvc.GetBasket(user.Name);

                var basket = await _basketSvc.SetQuantities(user.Name, mybasket);
                await _basketSvc.UpdateBasket(vm);
                return RedirectToAction("Index", "Basket");
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
                return RedirectToAction("Index", "Basket", new { errorMsg = ViewBag.BasketInoperativeMsg });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(Dictionary<string, int> quantities, string action)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
               // var basket = await _basketSvc.SetQuantities(user.UserName, quantities);
                if (action == "[ Checkout ]")
                {
                    return View();
                }
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToBasket(CatalogItemViewModel productDetails)
        {
            try
            {
                if (productDetails?.Id != null)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _basketSvc.AddItemToBasket(user.Name, productDetails.Name, productDetails.PictureUri, productDetails.Price, 1);
                    return RedirectToAction("Index", "Basket");
                }
                return RedirectToAction("Index", "Catalog");
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
                return RedirectToAction("Index", "Catalog", new { errorMsg = ViewBag.BasketInoperativeMsg });
            }


        }

        private void HandleBrokenCircuitException()
        {
            ViewBag.BasketInoperativeMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
////using Microsoft.eShopWeb.ApplicationCore.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.eShopWeb.Web.ViewModels;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.eShopWeb.Infrastructure.Identity;
//using System;
//using System.Collections.Generic;
//using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.eShopWeb.Web.Interfaces;

//namespace Microsoft.eShopWeb.Web.Controllers
//{
//    [Route("[controller]/[action]")]
//    public class BasketController : Controller
//    {
//        private readonly IBasketService _basketService;
//        private readonly IUriComposer _uriComposer;
//        //private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly IAppLogger<BasketController> _logger;
//        private readonly IOrderService _orderService;
//        private readonly IBasketViewModelService _basketViewModelService;

//        public BasketController(IBasketService basketService,
//            IBasketViewModelService basketViewModelService,
//            IOrderService orderService,
//            IUriComposer uriComposer,
//            //SignInManager<ApplicationUser> signInManager,
//            IAppLogger<BasketController> logger)
//        {
//            _basketService = basketService;
//            _uriComposer = uriComposer;
//            //_signInManager = signInManager;
//            _logger = logger;
//            _orderService = orderService;
//            _basketViewModelService = basketViewModelService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            var basketModel = await GetBasketViewModelAsync();

//            return View(basketModel);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Index(Dictionary<string, int> items)
//        {
//            var basketViewModel = await GetBasketViewModelAsync();
//            await _basketService.SetQuantities(basketViewModel.Id, items);

//            return View(await GetBasketViewModelAsync());
//        }


//        // POST: /Basket/AddToBasket
//        [HttpPost]
//        public async Task<IActionResult> AddItemToBasket(CatalogItemViewModel productDetails)
//        {
//            if (productDetails?.Id == null)
//            {
//                return RedirectToAction("Index", "Catalog");
//            }
//            var basketViewModel = await GetBasketViewModelAsync();

//            await _basketService.AddItemToBasket(User.Identity.Name, basketViewModel.Id, productDetails.Id, productDetails.Price, 1);

//            return RedirectToAction("Index");
//        }

//        [HttpPost]
//        [Authorize]
//        public async Task<IActionResult> Checkout(Dictionary<string, int> items)
//        {
//            var basketViewModel = await GetBasketViewModelAsync();
//            await _basketService.SetQuantities(basketViewModel.Id, items);

//            await _orderService.CreateOrderAsync(basketViewModel.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));

//            await _basketService.DeleteBasketAsync(basketViewModel.Id);

//            return View("Checkout");
//        }

//        private async Task<BasketViewModel> GetBasketViewModelAsync()
//        {
//            //if (_signInManager.IsSignedIn(HttpContext.User))
//            //{
//            //    return await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
//            //}
//            //string anonymousId = GetOrSetBasketCookie();
//            //return await _basketViewModelService.GetOrCreateBasketForUser(anonymousId);
//            return null;
//        }

//        private string GetOrSetBasketCookie()
//        {
//            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
//            {
//                return Request.Cookies[Constants.BASKET_COOKIENAME];
//            }
//            string anonymousId = Guid.NewGuid().ToString();
//            var cookieOptions = new CookieOptions();
//            cookieOptions.Expires = DateTime.Today.AddYears(10);
//            Response.Cookies.Append(Constants.BASKET_COOKIENAME, anonymousId, cookieOptions);
//            return anonymousId;
//        }
//    }
//}