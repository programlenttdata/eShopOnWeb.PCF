using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.ViewModels;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Microsoft.eShopWeb.Web.Services
{
    public class GetCatalogCommand : HystrixCommand<Catalog>
    {
        private int PageIndex { get; set; }
        private int ItemsPage { get; set; }
        private int? BrandId { get; set; }
        private int? TypeId { get; set; }
        private readonly Func<int, int, int?, int?, Task<Catalog>> GetCatalogFn;
        private readonly Func<int, int, int?, int?, Task<Catalog>> GetCatalogFallbackFn;

        public GetCatalogCommand(Func<int, int, int?, int?, Task<Catalog>> getCatalogFn,
                                 Func<int, int, int?, int?, Task<Catalog>> getCatalogFallbackFn,
                                 int pageIndex, 
                                 int itemsPage, 
                                 int? brandId, 
                                 int? typeId) 
        : base(HystrixCommandGroupKeyDefault.AsKey("CatalogGroup"))
        {
            PageIndex = pageIndex;
            ItemsPage = itemsPage;
            BrandId = brandId;
            TypeId = typeId;
            GetCatalogFn = getCatalogFn;
            GetCatalogFallbackFn = getCatalogFallbackFn;
        }

        protected override async Task<Catalog> RunAsync() => await GetCatalogFn(PageIndex, ItemsPage, BrandId, TypeId);
        protected override async Task<Catalog> RunFallbackAsync() => await GetCatalogFallbackFn(PageIndex, ItemsPage, BrandId, TypeId);
    }
}