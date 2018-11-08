using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
using Ordering.API.Application.Commands;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    using Domain.AggregatesModel.OrderAggregate;
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
    using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    // Regular CommandHandler
    public class PlaceOrderCommandHandler
        : IRequestHandler<PlaceOrderCommand, OrderResponse>
            
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        // Using DI to inject infrastructure persistence Repositories
        public PlaceOrderCommandHandler(IMediator mediator, IOrderRepository orderRepository, IIdentityService identityService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            //_identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<OrderResponse> Handle(PlaceOrderCommand message, CancellationToken cancellationToken)
        {
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            var address = new Address(message.Street, message.City, message.State, message.Country, message.ZipCode);
            var order = new Order(message.UserId, message.UserName, address, message.CardTypeId, message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration);
            
            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

             _orderRepository.Add(order);
            var result = await _orderRepository.UnitOfWork.SaveEntitiesAsync();
            var orderResponse = new OrderResponse {OrderId = order.Id, Response = result};
            return orderResponse;
        }
    }


    // Use for Idempotency in Command process
    public class PlaceOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateOrderCommand, bool>
    {
        public PlaceOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for creating order.
        }
    }
}
