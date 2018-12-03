using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Microsoft.eShopWeb.Web.ViewModels
{
    public class Basket : PageModel
    {
        // Use property initializer syntax.
        // While this is often more useful for read only 
        // auto implemented properties, it can simplify logic
        // for read/write properties.
        public int Id { get; set; }

        [BindProperty(SupportsGet = false)]
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        [BindProperty(SupportsGet = false)]
        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}
