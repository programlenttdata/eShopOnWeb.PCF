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
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                if (String.IsNullOrWhiteSpace(user.Id))
                {
                    return Content("Please Login");
                }
                var vm = await _basketSvc.GetBasket(user.Id);

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
        public async Task<IActionResult> UpdateBasket(Dictionary<string, int> items)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var vm = await _basketSvc.GetBasket(user.Id);

                var basket = await _basketSvc.SetQuantities(user.Id, items);
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
        public async Task<IActionResult> Checkout(Dictionary<string, int> items, string action)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var basket = await _basketSvc.SetQuantities(user.Id, items);
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

        [HttpPost, HttpPut]
        public async Task<IActionResult> AddItemToBasket(CatalogItemViewModel productDetails)
        {
            try
            {
                if (productDetails?.Id != null)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    if (String.IsNullOrWhiteSpace(user.Id) )
                    {
                        return Content("Please Login");
                    }
                    await _basketSvc.AddItemToBasket(user.Id, productDetails.Id.ToString(), productDetails.Name, productDetails.PictureUri, productDetails.Price, 1);
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

