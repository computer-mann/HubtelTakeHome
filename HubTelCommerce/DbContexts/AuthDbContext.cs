using HubTelCommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HubTelCommerce.DbContexts
{
    public class AuthDbContext:IdentityDbContext<Customer>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> db):base(db)
        {
            
        }
    }
}
