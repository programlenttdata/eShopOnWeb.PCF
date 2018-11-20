using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.Comm;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly string _remoteServiceBaseUrl;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _catalogItemRepository;
        private readonly HttpClient _httpClient;
        public OrderService(IAsyncRepository<CatalogItem> catalogItemRepository , IAsyncRepository<Basket> basketRepository, HttpClient httpClient, ILogger<OrderService> logger )
        {
            _catalogItemRepository = catalogItemRepository;
            _basketRepository = basketRepository;
            _httpClient = httpClient;
            _remoteServiceBaseUrl = httpClient.BaseAddress.ToString();
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(int basketId, Address shippingAddress, string UserId)
        {
        
            var basket = await _basketRepository.GetByIdAsync(basketId);
            string uri = API.Order.AddNewOrder(_remoteServiceBaseUrl);
            Guard.Against.NullBasket(basketId, basket);
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var catalogItem = await _catalogItemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity);
                items.Add(orderItem);
            }
           
            var order = new PlaceOrder(basket.Items, UserId, UserId,shippingAddress.City, shippingAddress.Street, shippingAddress.State, shippingAddress.Country, shippingAddress.ZipCode );
             var response = await _httpClient.PostAsJsonAsync(uri,order);
            return  await response.Content.ReadAsAsync<OrderResponse>();

        }
    }
}
