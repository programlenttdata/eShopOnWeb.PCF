using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using WebMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.Annotations;


namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public class Order
    {
        public string OrderNumber { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public decimal Total { get; set; }

        public string Description { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }

        public string ZipCode { get; set; }
        [Required]
        [DisplayName("Card number")]
        public string CardNumber { get; set; }
        [Required]
        [DisplayName("Cardholder name")]
        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        [CardExpiration(ErrorMessage = "The card is expired"), Required]
        [DisplayName("Card expiration")]
        public string CardExpirationShort { get; set; }
        [Required]
        [DisplayName("Card security number")]
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public List<SelectListItem> ActionCodeSelectList =>
           GetActionCodesByCurrentState();

        // See the property initializer syntax below. This
        // initializes the compiler generated field for this
        // auto-implemented property.
        public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

        [Required]
        public Guid RequestId { get; set; }


        public void CardExpirationShortFormat()
        {
            CardExpirationShort = CardExpiration.ToString("MM/yy");
        }

        public void CardExpirationApiFormat()
        {
            var month = CardExpirationShort.Split('/')[0];
            var year = $"20{CardExpirationShort.Split('/')[1]}";

            CardExpiration = new DateTime(int.Parse(year), int.Parse(month), 1);
        }

        private List<SelectListItem> GetActionCodesByCurrentState()
        {
            var actions = new List<OrderProcessAction>();
            switch (Status?.ToLower())
            {
                case "paid":
                    actions.Add(OrderProcessAction.Ship);
                    break;
            }

            var result = new List<SelectListItem>();
            actions.ForEach(action =>
            {
                result.Add(new SelectListItem { Text = action.Name, Value = action.Code });
            });

            return result;
        }
    }

    public enum CardType
    {
        AMEX = 1
    }
}




namespace WebMVC.Models
{
    public class BasketDTO
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }

        public string ZipCode { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardHolderName { get; set; }

        [Required]
        public DateTime CardExpiration { get; set; }

        [Required]
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        [Required]
        public Guid RequestId { get; set; }
    }
}

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CardExpirationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var monthString = value.ToString().Split('/')[0];
            var yearString = $"20{value.ToString().Split('/')[1]}";
            // Use the 'out' variable initializer to simplify 
            // the logic of validating the expiration date
            if ((int.TryParse(monthString, out var month)) &&
                (int.TryParse(yearString, out var year)))
            {
                DateTime d = new DateTime(year, month, 1);

                return d > DateTime.UtcNow;
            }
            else
            {
                return false;
            }
        }
    }
}
namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public int Units { get; set; }

        public string PictureUrl { get; set; }
    }
}
