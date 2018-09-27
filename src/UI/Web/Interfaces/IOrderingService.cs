using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Infrastructure.Identity;

namespace Microsoft.eShopWeb.Web.Services

{
    public interface IOrderingService
    {
        Task<List<Order>> GetMyOrders(string UserId);
        Task<Order> GetOrder(string  UserId, string orderId);
    }
}
