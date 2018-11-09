using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.Web.Models
{
    public class OrderDTO
    {
        [Required]
        public string OrderNumber { get; set; }
    }
}