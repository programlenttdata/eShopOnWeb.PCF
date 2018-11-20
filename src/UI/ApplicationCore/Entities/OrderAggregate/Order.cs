using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using System;
using Bogus;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate
{


    public class Order : BaseEntity, IAggregateRoot
    {
        private Order()
        {
            // required by EF
        }

        public Order(string buyerId, Address shipToAddress, List<OrderItem> items)
        {
            Guard.Against.NullOrEmpty(buyerId, nameof(buyerId));
            Guard.Against.Null(shipToAddress, nameof(shipToAddress));
            Guard.Against.Null(items, nameof(items));

            BuyerId = buyerId;
            ShipToAddress = shipToAddress;
            _orderItems = items;
        }
        public string BuyerId { get; private set; }

        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; private set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        // Using List<>.AsReadOnly() 
        // This will create a read only wrapper around the private list so is protected against "external updates".
        // It's much cheaper than .ToList() because it will not have to copy all items in a new collection. (Just one heap alloc for the wrapper instance)
        //https://msdn.microsoft.com/en-us/library/e78dcd75(v=vs.110).aspx 

        public decimal Total()
        {
            var total = 0m;
            foreach (var item in _orderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total;
        }
    }
}

namespace NTTData.eShopMonoToMicro.Entities.Orders
{
    public static class BasketItemExtensions
    {
        public static IEnumerable<OrderItemDTO> ToOrderItemsDTO(this IEnumerable<BasketItem> basketItems)
        {
            foreach (var item in basketItems)
            {
                yield return item.ToOrderItemDTO();
            }
        }

        public static OrderItemDTO ToOrderItemDTO(this BasketItem item)
        {
            return new OrderItemDTO()
            {
                ProductId = item.Id,
                ProductName = item.CatalogItemId.ToString(),
                PictureUrl = "",
                UnitPrice = item.UnitPrice,
                Units = item.Quantity
            };
        }
    }


    public class OrderItemDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public int Units { get; set; }

        public string PictureUrl { get; set; }
    }

    public class CreateOrderCommand
    {
        private readonly IEnumerable<OrderItemDTO> _orderItems;
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;
        public CreateOrderCommand(string Username, string buyerId, Address address, IReadOnlyCollection<BasketItem> basketItems)
        {
            
            //string userId, string userName, string city, string street, string state, string country, string zipcode,
            //string cardNumber, string cardHolderName, DateTime cardExpiration,
            //string cardSecurityNumber, int cardTypeId
            List<BasketItem> basketList = new List<BasketItem>(basketItems);
            _orderItems = BasketItemExtensions.ToOrderItemsDTO(basketList);
            UserId = buyerId; //buyerId is actually an email
            UserName = Username;
            City = address.City;
            Street = address.Street;
            State = address.State;
            Country = address.Country;
            ZipCode = address.ZipCode;
            
            CardTypeId = 1;
        }

    }

}