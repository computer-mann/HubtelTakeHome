using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HubTelCommerce.Models;
using HubTelCommerce.DbContexts;

namespace HubTelCommerce.Data
{
    public class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<UserManager<Customer>>();

            var user = new Customer
            {
                
                Email = "prince@example.com",
                UserName = "prince",
                EmailConfirmed = true,
            };
            var user2 = new Customer
            {

                Email = "evans@example.com",
                UserName = "evans",
                EmailConfirmed = true,
            };


            if (!userManager.Users.Any())
            {
              await userManager.CreateAsync(user,"asdf");
              await userManager.CreateAsync(user2, "asdf");

            }

           
        }

        
    }
}
