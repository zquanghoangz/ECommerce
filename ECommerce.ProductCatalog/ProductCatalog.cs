using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        IProductRepository _repository;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product product)
        {
            await _repository.AddProduct(product);
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            return (await _repository.GetAllProducts()).ToArray();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]{
            new ServiceReplicaListener(context =>
            new FabricTransportServiceRemotingListener(context, this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repository = new ServiceFabricProductResponsitory(StateManager);

            var products = new List<Product> {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Surface book",
                    Description = "This is Microsoft's laptop",
                    Availablity = 30,
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "MacBook Air",
                    Description = "This is Apple's laptop",
                    Availablity = 15,
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Dell XYZ",
                    Description = "This is Dell's laptop",
                    Availablity = 10,
                },
};

            foreach (var item in products)
            {
                await _repository.AddProduct(item);
            }

            var all = await _repository.GetAllProducts();
        }
    }
}
