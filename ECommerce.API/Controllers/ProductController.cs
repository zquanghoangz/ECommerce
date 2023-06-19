using ECommerce.API.Models;
using ECommerce.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductCatalogService _productCatalogService;

        public ProductController()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            _productCatalogService = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/ECommerce/ProductCatalog"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync()
        {
            var allProducts = await _productCatalogService.GetAllProductsAsync();


            return allProducts.Select(x => new ApiProduct
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsAvailable = x.Availability > 0,
            });
        }

        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product)
        {
            var newPRoduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = 100
            };

            await _productCatalogService.AddProductAsync(newPRoduct);
        }
    }
}