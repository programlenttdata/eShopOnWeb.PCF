using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(int basketId, Address shippingAddress, string Username);
    }
}
