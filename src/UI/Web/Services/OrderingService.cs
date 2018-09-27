using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Comm;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.Services;
using System.Text;

namespace Microsoft.eShopWeb.Web.Services
{
    public class OrderingService   : IOrderingService
    {
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public OrderingService(IOptionsSnapshot<AppSettings> settings, HttpClient httpClient, ILogger<CatalogService> logger)
        {
            _remoteServiceBaseUrl = $"{settings.Value.PurchaseUrl}/api/v1/o/orders";
            _settings = settings;
            _apiClient = httpClient;
        }

        async public Task<Order> GetOrder(string  userId, string id)
        {
            var getOrderUri = API.Order.GetOrder(_remoteServiceBaseUrl, id);
            var dataString = await _apiClient.GetStringAsync(getOrderUri);
            var response = JsonConvert.DeserializeObject<Order>(dataString);
            return response;
        }

  public async Task PlaceOrderAsync(Order  order)
        {
            var getOrderUri = API.Order.PlaceOrder(_remoteServiceBaseUrl);
            var json = JsonConvert.SerializeObject(order);
            var orderContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(getOrderUri, orderContent);
            response.EnsureSuccessStatusCode();
        }
        

        async public Task<List<Order>> GetMyOrders(string  userId)
        {
            var allMyOrdersUri = API.Order.GetAllMyOrders(_remoteServiceBaseUrl,userId);
            var dataString = await _apiClient.GetStringAsync(allMyOrdersUri);
            var response = JsonConvert.DeserializeObject<List<Order>>(dataString);

            return response;
        }

                
        

        void SetFakeIdToProducts(Order order)
        {
            var id = 1;
            //order.OrderItems.ForEach(x => { x.ProductId = id; id++; });
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.GetTokenAsync("access_token");
        }        
    }
}
