using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.Models;
using Microsoft.eShopWeb.Infrastructure.Identity;

namespace Microsoft.eShopWeb.Web.Services
{
    public interface IOrderingService
    {
        Task<List<Order>> GetMyOrders(ApplicationUser user);
        Task<Order> GetOrder(ApplicationUser user, string orderId);
        Task CancelOrder(string orderId);
        Task ShipOrder(string orderId);
        Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
        BasketDTO MapOrderToBasket(Order order);
        void OverrideUserInfoIntoOrder(Order original, Order destination);
    }
}
