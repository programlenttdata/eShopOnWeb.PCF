using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Common.CommandBus
{
    public interface ICommandBus
    {
        Task SendAsync<T>(T command) where T : IntegrationCommand;

    }
}
