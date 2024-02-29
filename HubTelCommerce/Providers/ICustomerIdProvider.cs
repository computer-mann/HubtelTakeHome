namespace HubTelCommerce.Providers
{
    public interface ICustomerIdProvider
    {
        
        public string GetCustomerId(IHttpContextAccessor httpContext);
    }
}
