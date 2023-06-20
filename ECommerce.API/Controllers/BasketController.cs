using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using UserActor.Interfaces;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasketController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ApiBasket> GetAsync(string userId)
        {
            var actor = GetActor(userId);

            var products = await actor.GetBasket();

            return new ApiBasket
            {
                UserId = userId,
                Items = products.Select(x => new ApiBasketItem
                {
                    ProductId = x.ProductId.ToString(),
                    Quantity = x.Quantity,
                }).ToArray(),
            };
        }

        [HttpPost("{userId}")]
        public async Task AddAsync(string userId, [FromBody] ApiBasketAddRequest request)
        {
            var actor = GetActor(userId);
            await actor.AddToBasket(request.ProductId, request.Quantity);
        }

        [HttpDelete("{userId}")]
        public async Task DeleteAsync(string userId)
        {
            var actor = GetActor(userId);
            await actor.ClearBasket();
        }

        private IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new Microsoft.ServiceFabric.Actors.ActorId(userId), new Uri("fabric:/ECommerce/UserActorService"));
        }
    }
}