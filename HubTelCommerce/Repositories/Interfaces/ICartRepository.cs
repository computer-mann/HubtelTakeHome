using HubTelCommerce.Models;

namespace HubTelCommerce.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> AddOrUpdateItemsInCartAsync(Cart Cart);
        Task DeleteItemFromCartAsync(string cartId, int itemId, string customerId);
        Task<IEnumerable<Cart>> GetAllCartItemsAsync(string cartId,string customerId,string? itemName);
        Task<Cart?> GetSingleCartItemAsync(string cartId, int itemId, string customerId);
        
    }
}
/*
 * 
 * 
Provide an endpoint to Add items to cart, with specified quantity.Adding similar items (same item ID) should increase the quantity – POST 


Provide an endpoint to remove an item from cart - DELETE verb 


Provide an endpoint list all cart items (with filters => phoneNumbers, time, quantity, item – GET 


Provide endpoint to get single item - GET
 */