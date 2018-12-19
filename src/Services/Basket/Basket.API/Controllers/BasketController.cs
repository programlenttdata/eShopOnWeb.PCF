using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Common.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Basket.API.ViewModel;
using Microsoft.eShopOnContainers.Services.Basket.API.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Basket.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.ConfigServer;
//using Pivotal.Extensions.Configuration.ConfigServer;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
    [Route("api/v1/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identitySvc;
        private readonly IEventBus _eventBus;
        private IConfigurationRoot Config { get; set; }
        
        public BasketController(IBasketRepository repository,
            IConfigurationRoot config,
            IIdentityService identityService,
            IEventBus eventBus)
        {
            _repository = repository;
            _identitySvc = identityService;
            _eventBus = eventBus;
            Config = config;
        }

        // GET /id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var basket = await _repository.GetBasketAsync(id);
            if (basket == null)
            {
                return Ok(new CustomerBasket(id) { });
            }

            return Ok(basket);
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<BasketItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<BasketItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Items([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, [FromQuery] string ids = null)
        {
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    return GetItemsByIds(ids);
            //}

            //var totalItems = await _basketContext.CatalogItems
            //    .LongCountAsync();

            //var itemsOnPage = await _basketContext.CatalogItems
            //    .OrderBy(c => c.Name)
            //    .Skip(pageSize * pageIndex)
            //    .Take(pageSize)
            //    .ToListAsync();

            //itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<BasketItem>(
                pageIndex, pageSize, 10, new BasketItem[0]);

            return Ok(model);
        }

        // POST /value
        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]CustomerBasket value)
        {
           //Console.WriteLine(Config["PicBaseUrl"]);
           //Console.WriteLine(Config["CatalogQueueName"]);

            var basket = await _repository.UpdateBasketAsync(value);

            return Ok(basket);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Patch([FromBody]CustomerBasket basketPatch)
        {
            Console.WriteLine(Config["CatalogQueueName"]);
            var basket = await _repository.GetBasketAsync(basketPatch.BuyerId);
            foreach (var items in  basket.Items)
            {
                foreach (var patchItems in basketPatch.Items)
                {
                    if (items.ProductId == patchItems.ProductId)
                    {
                        patchItems.Id = items.Id;
                        patchItems.OldUnitPrice = items.OldUnitPrice;
                        patchItems.PictureUrl = items.PictureUrl;
                        patchItems.ProductName = items.ProductName;
                        patchItems.UnitPrice = items.UnitPrice;

                    }
                }

            }
            await _repository.UpdateBasketAsync(basketPatch);

            return Ok(basketPatch);
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var userId = _identitySvc.GetUserIdentity();


            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
                guid : basketCheckout.RequestId;
            
            var basket = await _repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest();
            }

            var userName = User.FindFirst(x => x.Type == "unique_name").Value;

            var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
                basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
                basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);
            // basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId,  basket  );

            // Once basket is checkout, sends an integration event to
            // ordering.api to convert basket to order and proceeds with
            // order creation process
            _eventBus.Publish(eventMessage);

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.Accepted)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {

            var response = await _repository.DeleteBasketAsync(id);
            if (response == false)
            {
                return BadRequest();
            }

            return Accepted();

        }


    }
}
