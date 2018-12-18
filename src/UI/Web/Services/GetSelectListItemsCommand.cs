using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.Web.ViewModels;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Microsoft.eShopWeb.Web.Services
{
    public class GetSelectListItemsCommand : HystrixCommand<IEnumerable<SelectListItem>>
    {
        private readonly Func<Task<IEnumerable<SelectListItem>>> GetItemsFn;
        private readonly Func<Task<IEnumerable<SelectListItem>>> GetItemsFallbackFn;

        public GetSelectListItemsCommand(Func<Task<IEnumerable<SelectListItem>>> getCatalogFn,
                                 Func<Task<IEnumerable<SelectListItem>>> getCatalogFallbackFn) 
        : base(HystrixCommandGroupKeyDefault.AsKey("SelectListItemGroup"))
        {
            GetItemsFn = getCatalogFn;
            GetItemsFallbackFn = getCatalogFallbackFn;
        }

        protected override async Task<IEnumerable<SelectListItem>> RunAsync() => await GetItemsFn();
        protected override async Task<IEnumerable<SelectListItem>> RunFallbackAsync() => await GetItemsFallbackFn();
    }
}