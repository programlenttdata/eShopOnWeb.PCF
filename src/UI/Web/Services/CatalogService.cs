using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.eShopWeb.Comm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.eShopWeb.Web.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly IRepository<ApplicationCore.Entities.CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogBrand> _brandRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IUriComposer _uriComposer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly string _remoteServiceBaseUrl;

        private readonly Dictionary<string, Catalog> CatalogCache;
        private readonly Dictionary<string, IEnumerable<SelectListItem>> DataCache;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{httpClient.BaseAddress}api/v1/catalog/";

            CatalogCache = new Dictionary<string, Catalog>();
            DataCache = new Dictionary<string, IEnumerable<SelectListItem>>();
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var catalog = await new GetCatalogCommand(GetCatalogFromServer, GetCatalogFromCache, pageIndex, itemsPage, brandId, typeId).ExecuteAsync();

            var vm = new CatalogIndexViewModel()
            {
                CatalogItems = catalog.Data.Select(i => new CatalogItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    PictureUri = i.PictureUri,
                    Price = i.Price
                }),
                Brands = await GetBrands(),
                Types = await GetTypes(),
                BrandFilterApplied = brandId ?? 0,
                TypesFilterApplied = typeId ?? 0,
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)catalog.Count / itemsPage)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return vm;
        }
 
        private async Task<Catalog> GetCatalogFromServer(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var uri = API.Catalog.GetAllCatalogItems(_remoteServiceBaseUrl, pageIndex, itemsPage, brandId, typeId);

            var responseString = await _httpClient.GetStringAsync(uri);

            var catalog = JsonConvert.DeserializeObject<Catalog>(responseString);
            var cachekey = pageIndex.ToString() + itemsPage.ToString() + (brandId.HasValue ? brandId.ToString() : "0") + (typeId.HasValue ? typeId.ToString() : "0");
            CatalogCache.Add(cachekey, catalog);

            return catalog;
        }

        private Task<Catalog> GetCatalogFromCache(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var cachekey = pageIndex.ToString() + itemsPage.ToString() + (brandId.HasValue ? brandId.ToString() : "0") + (typeId.HasValue ? typeId.ToString() : "0");
            return Task.FromResult(CatalogCache[cachekey]);
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            return await new GetSelectListItemsCommand(GetBrandsFromServer, GetBrandsFromCache).ExecuteAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetBrandsFromServer()
        {
            var uri = API.Catalog.GetAllBrands(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var brands = JArray.Parse(responseString);

            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }
            
            DataCache.Add("Brands", items);

            return items;
        }

        private Task<IEnumerable<SelectListItem>> GetBrandsFromCache()
        {
            return Task.FromResult(DataCache["Brands"]);
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            return await new GetSelectListItemsCommand(GetTypesFromServer, GetTypesFromCache).ExecuteAsync();

        }
        
        private async Task<IEnumerable<SelectListItem>> GetTypesFromServer()
        {
            var uri = API.Catalog.GetAllTypes(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var brands = JArray.Parse(responseString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }
            
            DataCache.Add("Types", items);
            
            return items;
        }

        private Task<IEnumerable<SelectListItem>> GetTypesFromCache()
        {
            return Task.FromResult(DataCache["Types"]);
        }
        
    }
}
