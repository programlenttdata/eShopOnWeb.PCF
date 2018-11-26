using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using WebMVC.Models;
using ApplicationUser = Microsoft.eShopWeb.Infrastructure.Identity.ApplicationUser;

namespace Microsoft.eShopWeb.Web.Interfaces
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
