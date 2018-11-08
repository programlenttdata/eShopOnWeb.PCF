using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;

namespace Catalog.API.Model
{
    public class CatalogItemRepository : EFRepository<CatalogItem>
    {
        public CatalogItemRepository(CatalogContext dbContext) : base(dbContext)
        {
        }
    }
}
