using HubTelCommerce.Models;
using HubTelCommerce.Providers;
using HubTelCommerce.Repositories.Interfaces;
using HubTelCommerce.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HubTelCommerce.Controllers
{
    [ApiController]
    [Route("/cart")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<CartController> logger;
        private readonly ICustomerIdProvider customerIdProvider;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartController(ICartRepository cartRepository, ILogger<CartController> logger, ICustomerIdProvider customerIdProvider, IHttpContextAccessor contextAccessor)
        {
            _cartRepository = cartRepository;
            this.logger = logger;
            this.customerIdProvider = customerIdProvider;
            _contextAccessor = contextAccessor;
        }
        [HttpGet("{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListCartItems(string cartId, [FromQuery]string? itemName)
        {
            try
            {
                if (string.IsNullOrEmpty(cartId))
                {
                    return BadRequest(new { Message = "cartId is required" });
                }
                var customerId = customerIdProvider.GetCustomerId(_contextAccessor);
                var itemList = await _cartRepository.GetAllCartItemsAsync(cartId, customerId,itemName);
                if(!itemList.Any())
                {
                    return NotFound(new {Message="Problem with the Query parameters."});
                }
                return Ok(itemList);
            }catch (Exception ex)
            {
                logger.LogError("ListCartItems operation failed with reason: {exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCartItems(AddCartItemViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            try
            {
                var cart = new Cart()
                {
                    CustomerId = customerIdProvider.GetCustomerId(_contextAccessor),
                    CartId = viewModel.CartId,
                    ItemName = viewModel.ItemName,
                    ItemId = viewModel.ItemId,
                    UnitPrice = viewModel.UnitPrice,
                    Quantity = viewModel.Quantity,
                };
                var result = await _cartRepository.AddOrUpdateItemsInCartAsync(cart);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError("AddCartItems operation failed with reason: {exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
           
            
        }
        [HttpDelete("{cartId}/product/{itemId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveCartItem(string cartId,int itemId)
        {
            try
            {
                var customerId = customerIdProvider.GetCustomerId(_contextAccessor);
                await _cartRepository.DeleteItemFromCartAsync(cartId, itemId, customerId);
                return NoContent();
            }catch(InvalidOperationException e)
            {
                logger.LogError("RemoveCartItem Operation Failed with Reason: {exception}", e);
                return NotFound(new { e.Message});
            }
            
            
        }
        [HttpGet("{cartId}/product/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingleCartItem(string cartId, int itemId)
        {
            try
            {
                var customerId = customerIdProvider.GetCustomerId(_contextAccessor);
                var cartItem =await _cartRepository.GetSingleCartItemAsync(cartId, itemId, customerId);
                if(cartItem == null) return NotFound(new {Message="Item or Cart does not exist. "});
                return Ok(cartItem);

            }
            catch(Exception e)
            {
                logger.LogError("GetSingleCartItem failed with reason: {exception}", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }
    }
}
