using HubTelCommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HubTelCommerce.DbContexts
{
    public class CustomerDbContext:IdentityDbContext<Customer>
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> db):base(db)
        {
            
        }
    }
}
