using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Bogus;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using NTTData.eShopMonoToMicro.Entities.Orders;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate
{
    public class PlaceOrder

    {
        public List<OrderItemDTO> _orderItems;
        private Basket basket;
        private string userId1;
        private string userId2;

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

      
        public  PlaceOrder(IReadOnlyCollection<BasketItem> basketItems, string userId, string userName, string city,
            string street, string state, string country, string zipcode,
             string cardHolderName = null, string cardNumber = null, DateTime? cardExpiration =null,
            string cardSecurityNumber = null, int? cardTypeId  = null) 
        {
            Randomizer.Seed = new Random(91239122);
            var faker = new Faker("en");
            
            _orderItems = basketItems.ToOrderItemsDTO().ToList();
            UserId = userId;
            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;

            CardNumber = cardNumber  ?? faker.Finance.CreditCardNumber();
            CardExpiration = cardExpiration ?? faker.Date.Future(4) ;
            CardHolderName =  cardHolderName ?? $"{faker.Name.FirstName()} {faker.Name.LastName()}";
            CardSecurityNumber = cardSecurityNumber ?? faker.Finance.CreditCardCvv();
            CardTypeId = cardTypeId ?? 1;
            ;
            
            
        }

    }

}