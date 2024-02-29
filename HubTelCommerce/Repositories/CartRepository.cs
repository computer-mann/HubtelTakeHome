using HubTelCommerce.DbContexts;
using HubTelCommerce.Models;
using HubTelCommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HubTelCommerce.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CommerceDbContext _dbContext;
        private readonly ILogger<CartRepository> logger;

        public CartRepository(CommerceDbContext dbContext,ILogger<CartRepository> logger)
        {
            _dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Cart> AddOrUpdateItemsInCartAsync(Cart cartModel)
        {
            EntityEntry<Cart> entityEntry;
            //create new cart if no such id in db
            if(string.IsNullOrEmpty(cartModel.CartId))
            {
                cartModel.CartId = Guid.NewGuid().ToString();
                entityEntry = await _dbContext.Carts.AddAsync(cartModel);
            }
            else
            {
                var cart = await _dbContext.Carts.Where(x => x.CartId == cartModel.CartId && x.ItemId == cartModel.ItemId).SingleOrDefaultAsync();
                if(cart == null)
                {
                    entityEntry = await _dbContext.Carts.AddAsync(cartModel);
                }
                else
                {
                    cart.Quantity+= cartModel.Quantity;
                    entityEntry =  _dbContext.Update(cart);
                }
            }
            await _dbContext.SaveChangesAsync();
            return entityEntry.Entity;

        }
        
        public async Task DeleteItemFromCartAsync(string cartId,int itemId,string customerId)
        {
            
                var cart = await _dbContext.Carts.Where(x => x.CartId == cartId && x.ItemId == itemId && x.CustomerId == customerId).SingleAsync();
                 _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
                await Task.CompletedTask;
            
        }

        //come back to the filters
        public async Task<IEnumerable<Cart>> GetAllCartItemsAsync(string cartId, string customerId,string? itemName)
        {
            var query = _dbContext.Carts.Where(x => x.CartId == cartId && x.CustomerId == customerId);
            if (!string.IsNullOrEmpty(itemName))
            {
               return await query.Where(x=>x.ItemName == itemName).ToListAsync();
            }
            return await query.ToListAsync();

        }

        public async Task<Cart?> GetSingleCartItemAsync(string cartId, int itemId, string customerId)
        {
            return await _dbContext.Carts.Where(x => x.CartId == cartId && x.ItemId== itemId && x.CustomerId == customerId).SingleOrDefaultAsync();
        }

        
    }
}
