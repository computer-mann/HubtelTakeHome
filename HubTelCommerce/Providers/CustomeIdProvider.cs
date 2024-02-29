
using System.Security.Claims;

namespace HubTelCommerce.Providers
{
    public class CustomerIdProvider : ICustomerIdProvider
    {
        public string GetCustomerId(IHttpContextAccessor httpContext)
        {
           return httpContext.HttpContext.User.FindFirst(ClaimTypes.PrimarySid).Value;
        }
    }
}
