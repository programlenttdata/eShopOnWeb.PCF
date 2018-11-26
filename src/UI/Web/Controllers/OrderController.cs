using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Services;
using Order = Microsoft.eShopOnContainers.WebMVC.ViewModels.Order;

namespace Microsoft.eShopWeb.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderingService  _orderService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        public OrderController(IOrderingService  orderingService, IOrderRepository orderRepository, IIdentityParser<ApplicationUser> appUserParser)
        {
            _appUserParser = appUserParser;
            _orderService = orderingService;
            _orderRepository = orderRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var orders =  await _orderService.GetMyOrders(user);
            return GenerateOrderViewModel(orders);
        }

        private ViewResult GenerateOrderViewModel(IEnumerable<Microsoft.eShopOnContainers.WebMVC.ViewModels.Order> orders)
        {
            var viewModel = orders
                .Select(o => new OrderViewModel()
                {
                    OrderDate = o.Date,
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemViewModel()
                    {
                        Discount = 0,
                        PictureUrl = oi.PictureUrl,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        UnitPrice = oi.UnitPrice,
                        Units = oi.Units
                    }).ToList(),
                    OrderNumber = Convert.ToInt32(o.OrderNumber),
                    ShippingAddress = new Address(o.City, o.Country, o.State, o.Street, o.ZipCode),
                    Status = "Pending",
                    Total = o.Total
                });
            return View(viewModel);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Detail(int orderId)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var customerOrders =  await _orderService.GetMyOrders(user);
            var order = customerOrders.FirstOrDefault(o => o.OrderNumber == orderId.ToString());
            if (order == null)
            {
                return BadRequest("No such order found for this user.");
            }
            return GenerateOrderViewModel(new List<Order>() { order });
        }
    }
}
