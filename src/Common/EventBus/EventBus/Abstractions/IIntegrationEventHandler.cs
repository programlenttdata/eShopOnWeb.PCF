using Microsoft.eShopOnContainers.Common.EventBus.Events;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Common.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
