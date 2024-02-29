using System.ComponentModel.DataAnnotations;

namespace HubTelCommerce.ViewModels
{

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

}
