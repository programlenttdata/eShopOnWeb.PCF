using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.Models;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.eShopWeb.Infrastructure.Identity;



using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Web.Services
{
   public class BasketService : IBasketService
    {
        private readonly ILogger<BasketService> _logger;
        private readonly HttpClient _apiClient;
        private readonly string _basketUrl;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger)
        {
            _apiClient = httpClient;
            _logger = logger;

            _basketUrl = $"{httpClient.BaseAddress}api/v1/basket";

        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var uri = API.Basket.GetBasket(_basketUrl, user.Id);

            var responseString = await _apiClient.GetStringAsync(uri);

            return string.IsNullOrEmpty(responseString) ?
                new Basket() { BuyerId = user.Id } :
                JsonConvert.DeserializeObject<Basket>(responseString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var uri = API.Basket.UpdateBasket(_basketUrl);

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

        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {
            var uri = API.Purchase.UpdateBasketItem(_basketUrl);

            var basketUpdate = new
            {
                BasketId = user.Id,
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

        public async Task<Order> GetOrderDraft(string basketId)
        {
            var uri = API.Purchase.GetOrderDraft(_basketUrl, basketId);

            var responseString = await _apiClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<Order>(responseString);

            return response;
        }

        public async Task AddItemToBasket(ApplicationUser user, int productId)
        {
            var uri = API.Purchase.AddItemToBasket(_basketUrl);

            var newItem = new
            {
                BuyerId = System.String.IsNullOrEmpty(user.Id.ToString()) ? "1" : user.Id.ToString(),
                Items = new List<BasketItem>()
                {
                    new BasketItem()
                    {
                        ProductId =  productId.ToString(),
                        Quantity = 1

                    }

                }
            };

            //public List<BasketItem> Items { get; set; } = new List<BasketItem>();
            //public string BuyerId { get; set; }

            var basketContent = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);
        }
    }
}
