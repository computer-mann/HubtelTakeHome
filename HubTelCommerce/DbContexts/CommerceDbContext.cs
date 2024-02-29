using HubTelCommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace HubTelCommerce.DbContexts
{
    public class CommerceDbContext : DbContext
    {
        public CommerceDbContext(DbContextOptions<CommerceDbContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>().HasData(
              new()
              { 
                  Id=1,
                  CartId=Guid.NewGuid().ToString(), 
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
                CartId = Guid.NewGuid().ToString(),
                ItemId=1,
                UnitPrice=20,
                ItemName="Laptop",
                Quantity=10
              }
          );
        }
    }
}