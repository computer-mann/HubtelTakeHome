using HubTelCommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace HubTelCommerce.DbContexts
{
    public class CommerceDbContext : DbContext
    {
        public CommerceDbContext(DbContextOptions<CommerceDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cart> Carts { get; set; }

    }
}
