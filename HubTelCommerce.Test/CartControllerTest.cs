using FluentAssertions;
using HubTelCommerce.Controllers;
using HubTelCommerce.Models;
using HubTelCommerce.Providers;
using HubTelCommerce.Repositories.Interfaces;
using HubTelCommerce.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Threading.Tasks;

namespace HubTelCommerce.Test
{
    public class CartControllerTest
    {
        private  ILogger<CartController> _logger;
        private  ICartRepository _cartRepository;
        private CartController _cartController;
        private IHttpContextAccessor _httpContext;
        private ICustomerIdProvider _customerIdProvider;
        private List<Cart> carts;

        [SetUp]
        public void Setup()
        {
            _cartRepository = Substitute.For<ICartRepository>();
            _logger = Substitute.For<ILogger<CartController>>();
            _httpContext = Substitute.For<IHttpContextAccessor>();
            _customerIdProvider = Substitute.For<ICustomerIdProvider>();
            _cartController = new(_cartRepository, _logger, _customerIdProvider, _httpContext);
            carts = new List<Cart>
            {
                new()
              {
                  Id=1,
                  CartId="ddddd",
                  CustomerId= "5d1556a9-7975-4176-8905-b29b8ddd3097",
                  ItemId = 2,
                  UnitPrice = 20,
                  ItemName = "Phone",
                  Quantity = 15
              },
              new()
              {
                  Id = 2,
                CustomerId= "d71f983a-2f48-4509-be82-0405af685c6e",
                CartId = "aaaaa",
                ItemId=1,
                UnitPrice=20,
                ItemName="Laptop",
                Quantity=10
              }
            };
            
        }
        [TearDown]
        public void TearDown()
        {
            carts = new List<Cart>();
            _cartController = null;
            _customerIdProvider = null;
            _logger = null;
            _httpContext = null;
            _cartRepository = null;

        }

        [Test]
        public async Task ListCartItems_WhenCartContainsNoItems_ReturnsNotFoundObjectResult()
        {
            var expectedCustomerId = "50582307-f780-4406-b7b7-b89867048e86";
            var cartId = "cartId";
            var itemName = "itemName";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(expectedCustomerId);
            _cartRepository.GetAllCartItemsAsync("cartId", expectedCustomerId, itemName).Returns(Enumerable.Empty<Cart>());

            var result = await _cartController.ListCartItems(cartId, itemName);

            await _cartRepository.Received(1).GetAllCartItemsAsync(cartId, expectedCustomerId, itemName);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Test]
        public async Task ListCartItems_WhenCartContainsItems_ReturnsOkResult()
        {
            var expectedCustomerId = "50582307-f780-4406-b7b7-b89867048e86";
            var cartId = "cartId";
            var itemName = "itemName";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(expectedCustomerId);
            _cartRepository.GetAllCartItemsAsync("cartId", expectedCustomerId, itemName).Returns(carts);

            var result = await _cartController.ListCartItems(cartId, itemName);

            await _cartRepository.Received(1).GetAllCartItemsAsync(cartId, expectedCustomerId, itemName);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task ListCartItems_WhenCartIdIsNotProvided_ReturnsOkResult()
        {
            var expectedCustomerId = "50582307-f780-4406-b7b7-b89867048e86";
            var itemName = "itemName";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(expectedCustomerId);
            _cartRepository.GetAllCartItemsAsync(string.Empty, expectedCustomerId, itemName).Returns(carts);

            var result = await _cartController.ListCartItems(string.Empty, itemName);

            await _cartRepository.Received(0).GetAllCartItemsAsync(string.Empty, expectedCustomerId, itemName);
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Test]
        public async Task GetSingleCartItem_WhenCartContainsNoItems_ReturnsNotFoundObjectResult()
        {
            Cart cart = null;
            var customerId = "50582307-f780-4406-b7b7-b89867048e86";
            var itemId = 1;
            var cartId = "cartId";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(customerId);
            _cartRepository.GetSingleCartItemAsync(cartId,itemId, customerId).Returns(cart);

            var result = await _cartController.GetSingleCartItem(cartId, itemId);

            await _cartRepository.Received(1).GetSingleCartItemAsync(cartId, itemId, customerId);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Test]
        public async Task GetSingleCartItem_WhenCartHasItems_ReturnsOkObjectResult()
        {
            Cart cart = new Cart();
            var customerId = "50582307-f780-4406-b7b7-b89867048e86";
            var itemId = 1;
            var cartId = "cartId";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(customerId);
            _cartRepository.GetSingleCartItemAsync(cartId, itemId, customerId).Returns(cart);

            var result = await _cartController.GetSingleCartItem(cartId, itemId);

            await _cartRepository.Received(1).GetSingleCartItemAsync(cartId, itemId, customerId);
            result.Should().BeOfType<OkObjectResult>();

        }
        [Test]
        public async Task RemoveCartItem_WhenParamsProvidedAreInDb_ReturnsNoContentResult()
        {
            var customerId = "50582307-f780-4406-b7b7-b89867048e86";
            var itemId = 1;
            var cartId = "cartId";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(customerId);
            _cartRepository.DeleteItemFromCartAsync(cartId, itemId, customerId).Returns(Task.CompletedTask);

            var result = await _cartController.RemoveCartItem(cartId, itemId);

            await _cartRepository.Received(1).DeleteItemFromCartAsync(cartId, itemId, customerId);
            result.Should().BeOfType<NoContentResult>();
        }
        [Test]
        public async Task RemoveCartItem_ParamsProvidedAreNotInDb_ReturnsNotFoundObjectResult()
        {
            
            var customerId = "50582307-f780-4406-b7b7-b89867048e86";
            var itemId = 1;
            var cartId = "cartId";
            _customerIdProvider.GetCustomerId(_httpContext).Returns(customerId);
            _cartRepository.DeleteItemFromCartAsync(cartId, itemId, customerId).Throws<InvalidOperationException>();

            var result = await _cartController.RemoveCartItem(cartId, itemId);

            await _cartRepository.Received(1).DeleteItemFromCartAsync(cartId, itemId, customerId);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task AddCartItems_WhenCartModelRequirementIsFullyProvided_ReturnsOkObjectResult()
        {
            _customerIdProvider.GetCustomerId(_httpContext).Returns("customerId");
            var cartVm = new AddCartItemViewModel
            {
                UnitPrice = 1,
                Quantity = 1,
                CartId = "cartId",
                ItemId = 1,
                ItemName = "itemName",
            };
            var cart = new Cart
            {
                CartId = cartVm.CartId,
                CustomerId = _customerIdProvider.GetCustomerId(_httpContext),
                ItemId = cartVm.ItemId,
                ItemName = cartVm.ItemName,
                Quantity = cartVm.Quantity,
                UnitPrice = cartVm.UnitPrice,
            };
            _cartRepository.AddOrUpdateItemsInCartAsync(cart).Returns(cart);

            var result =await _cartController.AddCartItems(cartVm);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task AddCartItems_WhenCartIdModelRequirementIsNotProvided_ReturnsBadRequestResult()
        {
            _customerIdProvider.GetCustomerId(_httpContext).Returns("customerId");
            var cartVm = new AddCartItemViewModel
            {
                UnitPrice = 20,
                Quantity = 0,
                CartId = "cartId",
                ItemId = 1,
                ItemName = "itemName",
            };
            _cartController.ModelState.AddModelError("TestSession", "Failure");

            _cartRepository.AddOrUpdateItemsInCartAsync(new Cart()).Returns(new Cart());

            var result = await _cartController.AddCartItems(cartVm);

            await _cartRepository.Received(0).AddOrUpdateItemsInCartAsync(new Cart());
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }

}