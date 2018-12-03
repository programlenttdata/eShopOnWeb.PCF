
using Microsoft.eShopWeb.Web.Models;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.eShopWeb.Infrastructure.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IBasketService
    {
        Task<Basket> GetBasket(string user);
        Task AddItemToBasket(string user, string productName, string pictureUri, decimal unitPrice, int quantity);
        Task<Basket> UpdateBasket(Basket basket);
        Task Checkout(BasketDTO basket);
        Task<Basket> SetQuantities(string userName, Dictionary<string, int> quantities);
        Task<Order> GetOrderDraft(string basketId);

    }

    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}

