using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Infrastructure.Identity;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(int basketId, Address shippingAddress, string Username);
        Task<List<Order>> GetMyOrders(ApplicationUser user);
         Task<Order> GetOrder(ApplicationUser user, string orderId);
    }
}
