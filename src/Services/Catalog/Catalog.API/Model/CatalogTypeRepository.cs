using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Catalog.API.Extensions;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Catalog.API.Model
{
    public class CatalogTypeRepository : EFRepository<CatalogType>
    {
        public CatalogTypeRepository(CatalogContext dbContext) : base(dbContext)
        {
        }
    }
}
