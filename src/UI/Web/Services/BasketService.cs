//using System;
//using Microsoft.eShopWeb.Web.ViewModels;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.eShopWeb.Web.Models;
//using Microsoft.eShopWeb.Web.Interfaces;
//using Microsoft.eShopWeb.Infrastructure.Services;
//using Microsoft.eShopWeb.Infrastructure.Identity;
//using Microsoft.EntityFrameworkCore.Internal;
//using Microsoft.Extensions.Logging;

//namespace Microsoft.eShopWeb.Web.Services
//{
//   public class BasketService : IBasketService
//    {
//        private readonly ILogger<BasketService> _logger;
//        private readonly HttpClient _apiClient;
//        private readonly string _basketUrl;
//      //  private readonly string _basketByPassUrl;

//        public BasketService(HttpClient httpClient, ILogger<BasketService> logger)
//        {
//            _apiClient = httpClient;
//            _logger = logger;

//            _basketUrl = $"{httpClient.BaseAddress}api/v1/basket";
//          //  _basketByPassUrl = $"{httpClient.BaseAddress}api/v1/b/basket";
//        }

//        public async Task<Basket> GetBasket(ApplicationUser user)
//        {

//            //if (String.IsNullOrWhiteSpace(user.Id.ToString()))
//            //{
//            //    user.Id = "1";
//            //}

//            var uri = API.Basket.GetBasket(_basketUrl,user.Name);

//            var responseString = await _apiClient.GetStringAsync(uri);

//            return string.IsNullOrEmpty(responseString) ?
//                new Basket() { BuyerId = user.Name } :
//                JsonConvert.DeserializeObject<Basket>(responseString);
//        }

//        public async Task<Basket> UpdateBasket(Basket basket)
//        {
//            var uri = API.Basket.UpdateBasket(_basketUrl);

//            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

//            var response = await _apiClient.PostAsync(uri, basketContent);

//            response.EnsureSuccessStatusCode();

//            return basket;
//        }

//        public async Task Checkout(BasketDTO basket)
//        {
//            var uri = API.Basket.CheckoutBasket(_basketUrl);
//            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

//            var response = await _apiClient.PostAsync(uri, basketContent);

//            response.EnsureSuccessStatusCode();
//        }

//        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
//        {
//            var uri = API.Purchase.UpdateBasketItem(_basketUrl);

//            var basketUpdate = new
//            {
//                BasketId = user.Id,
//                Updates = quantities.Select(kvp => new
//                {
//                    BasketItemId = kvp.Key,
//                    NewQty = kvp.Value
//                }).ToArray()
//            };

//            var basketContent = new StringContent(JsonConvert.SerializeObject(basketUpdate), System.Text.Encoding.UTF8, "application/json");

//            var response = await _apiClient.PutAsync(uri, basketContent);

//            response.EnsureSuccessStatusCode();

//            var jsonResponse = await response.Content.ReadAsStringAsync();

//            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
//        }

//        public async Task<Order> GetOrderDraft(string basketId)
//        {
//            var uri = API.Purchase.GetOrderDraft(_basketUrl, basketId);

//            var responseString = await _apiClient.GetStringAsync(uri);

//            var response = JsonConvert.DeserializeObject<Order>(responseString);

//            return response;
//        }

//        public async Task AddItemToBasket(ApplicationUser user, string productName, string pictureUri, decimal unitPrice, int quantity = 1)
//        {
//            var uri = API.Purchase.AddItemToBasket(_basketUrl);

//            var newItem = new
//            {
//                BuyerId = user.Name,
//                Items = new List<BasketItem>()
//                {
//                    new BasketItem()
//                    {
//                        ProductName =  productName,
//                        PictureUrl =  pictureUri,
//                        UnitPrice =  unitPrice,
//                        Quantity =   quantity

//                    }

//                }

//            };

//            var basketContent = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

//            var response = await _apiClient.PostAsync(uri, basketContent);
//        }
//    }
//}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
//using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Models;
using System.Linq;
//using Basket = Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Basket;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        //private readonly IAsyncRepository<Basket> _basketRepository;
        //private readonly IUriComposer _uriComposer;
        //private readonly IAppLogger<BasketService> _logger;
        //private readonly IRepository<CatalogItem> _itemRepository;

        private readonly ILogger<BasketService> _logger;
        private readonly HttpClient _apiClient;
        private readonly string _basketUrl;
        //  private readonly string _basketByPassUrl;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger)
        {
            _apiClient = httpClient;
            _logger = logger;

            _basketUrl = $"{httpClient.BaseAddress}api/v1/basket";
            //  _basketByPassUrl = $"{httpClient.BaseAddress}api/v1/b/basket";
        }

        //public BasketService(IAsyncRepository<Basket> basketRepository,
        //    IRepository<CatalogItem> itemRepository,
        //    IUriComposer uriComposer,
        //    IAppLogger<BasketService> logger)
        //{
        //    //_basketRepository = basketRepository;
        //    _uriComposer = uriComposer;
        //    this._logger = logger;
        //    //_itemRepository = itemRepository;
        //}

        public async Task AddItemToBasket(string userName, string productName, string pictureUri, decimal unitPrice, int quantity = 1) //int basketId, int catalogItemId, decimal price, int quantity)
        {
            var uri = API.Purchase.AddItemToBasket(_basketUrl);
            //var responseString = await _apiClient.GetStringAsync(basketId.ToString());
            var basket = await GetBasket(userName);
            //var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");
            //await _apiClient.PostAsync(uri, basketContent);

            //            var uri = API.Purchase.AddItemToBasket(_basketUrl);

            basket.Items.Add(new Web.ViewModels.BasketItem()
            {
                ProductName = productName,
                PictureUrl = pictureUri,
                UnitPrice = unitPrice,
                Quantity = quantity

            });

            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);
        }

        public async Task<Basket> GetBasket(string userName)
        {
            
            var uri = API.Basket.GetBasket(_basketUrl, userName);

            var responseString = await _apiClient.GetStringAsync(uri);

            return string.IsNullOrEmpty(responseString) ?
                new Basket() { BuyerId = userName } :
                JsonConvert.DeserializeObject<Basket>(responseString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var uri = API.Purchase.UpdateBasketItem(_basketUrl);

            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();

            return basket;
     
        }
        public async Task Checkout(BasketDTO basket)
        {
            var uri = API.Basket.CheckoutBasket(_basketUrl);
            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();
        }
        //public async Task<Basket> SetQuantities(string userName, Dictionary<string, int> quantities)
        //{
        //    return null;
        //}

        public async Task<Basket> SetQuantities(string userName, Dictionary<string, int> quantities)
        {
            var uri = API.Purchase.UpdateBasketItem(_basketUrl);

            var basketUpdate = new
            {
                BasketId = userName,
                Updates = quantities.Select(kvp => new
                {
                    BasketItemId = kvp.Key,
                    NewQty = kvp.Value
                }).ToArray()
            };

            var basketContent = new StringContent(JsonConvert.SerializeObject(basketUpdate), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PutAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
        }

        public async Task<Microsoft.eShopWeb.Web.ViewModels.Order> GetOrderDraft(string basketId)
        {
            return null;
        }

        //public async Task DeleteBasketAsync(int basketId)
        //{
        //    return;
        //    //var basket = await _basketRepository.GetByIdAsync(basketId);

        //    //await _basketRepository.DeleteAsync(basket);
        //}

        //public async Task<int> GetBasketItemCountAsync(string userName)
        //{
        //    return 0;
        //    //Guard.Against.NullOrEmpty(userName, nameof(userName));
        //    //var basketSpec = new BasketWithItemsSpecification(userName);
        //    //var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();
        //    //if (basket == null)
        //    //{
        //    //    _logger.LogInformation($"No basket found for {userName}");
        //    //    return 0;
        //    //}
        //    //int count = basket.Items.Sum(i => i.Quantity);
        //    //_logger.LogInformation($"Basket for {userName} has {count} items.");
        //    //return count;
        //}

        //public async Task SetQuantities(int basketId, Dictionary<string, int> quantities)
        //{
            
        //    //Guard.Against.Null(quantities, nameof(quantities));
        //    var basket = await _basketRepository.GetByIdAsync(basketId);
        //    foreach (var item in basket.Items)
        //    {
        //        if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
        //        {
        //            _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
        //            item.Quantity = quantity;
        //        }
        //    }
        //    await _basketRepository.UpdateAsync(basket);
        //}

        //public async Task TransferBasketAsync(string anonymousId, string userName)
        //{
        //    return;
        //    //Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
        //    //Guard.Against.NullOrEmpty(userName, nameof(userName));
        //    //var basketSpec = new BasketWithItemsSpecification(anonymousId);
        //    //var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();
        //    //if (basket == null) return;
        //    //basket.BuyerId = userName;
        //    //await _basketRepository.UpdateAsync(basket);
        //}
    }
}