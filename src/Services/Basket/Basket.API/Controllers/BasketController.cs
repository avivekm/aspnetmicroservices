using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController:ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpsService;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpsService)
        {
            _repository = repository;
            _discountGrpsService = discountGrpsService;
        }

        [HttpGet("{userName}",Name ="GetBasket")]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach(var item in basket.Items)
            {
                var coupon = await _discountGrpsService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            return (await _repository.UpdateBasket(basket));
        }
        [HttpDelete("{userName}", Name = "DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }
}
