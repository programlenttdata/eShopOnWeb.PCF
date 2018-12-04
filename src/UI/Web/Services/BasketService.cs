using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Models;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly ILogger<BasketService> _logger;
        private readonly HttpClient _apiClient;
        private readonly string _basketUrl;
        private readonly string _purchaseUrl;

        private readonly string _bffUrl;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger)
        {
            _apiClient = httpClient;
            _logger = logger;

            _basketUrl = $"{httpClient.BaseAddress}api/v1/basket";

        }

        public async Task<Basket> GetBasket(string userId)
        {
            var uri = API.Basket.GetBasket(_basketUrl, userId);

            var responseString = await _apiClient.GetStringAsync(uri);

            return string.IsNullOrEmpty(responseString) ?
                new Basket() { BuyerId = userId } :
                JsonConvert.DeserializeObject<Basket>(responseString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var uri = API.Basket.UpdateBasket(_basketUrl);

            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PatchAsync(uri, basketContent);

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

        public async Task<Basket> SetQuantities(string userId, Dictionary<string, int> quantities)
        {
            var uri = API.Purchase.UpdateBasketItem(_basketUrl);

            var basketUpdate = new
            {
                BuyerId = userId,
                items = quantities
                    .Where(x => x.Value > 0)
                    .Select(kvp => new
                {
                    Productid = kvp.Key,
                    Quantity = kvp.Value
                }).ToArray()
            };

            var basketContent = new StringContent(JsonConvert.SerializeObject(basketUpdate), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PatchAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
        }

        public async Task<Order> GetOrderDraft(string basketId)
        {
            var uri = API.Purchase.GetOrderDraft(_purchaseUrl, basketId);

            var responseString = await _apiClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<Order>(responseString);

            return response;
        }


        public async Task AddItemToBasket(string userName, string productid, string productName, string pictureUri, decimal unitPrice, int quantity = 1) //int basketId, int catalogItemId, decimal price, int quantity)
        {
            var uri = API.Purchase.AddItemToBasket(_basketUrl);
            var basket = await GetBasket(userName);
            basket.Items.Add(new BasketItem()
            {
                Id = userName,
                ProductId =   productid,
                ProductName = productName,
                PictureUrl = pictureUri,
                UnitPrice = unitPrice,
                Quantity = quantity

            });

            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);
        }
    }
}
