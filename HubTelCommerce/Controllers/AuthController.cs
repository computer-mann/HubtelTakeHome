using HubTelCommerce.Models;
using HubTelCommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



namespace HubTelCommerce.Controllers
{
    
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<Customer> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if(user != null)
            {
                var result=await _userManager.CheckPasswordAsync(user, login.Password);
                if (result)
                {
                    var token = GenerateJwt(login.Username,user.Id);
                    HttpContext.Response.Headers.Add("auth-token", token);
                    return Ok(new { Message="Login successful" });
                }
                else
                {
                    return Unauthorized(new { Message = "Wrong password provided." });
                }
            }else
            {
                return NotFound(new {Message="Username does not exist"});
            }
            
        }

        private string GenerateJwt(string username,string userId)
        {
            var claims=new List<Claim>
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.PrimarySid,userId)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                                   claims: claims,
                                   audience: _configuration.GetSection("JwtSettings:Audience").Value,
                                   expires: DateTime.UtcNow.AddDays(1),
                                   signingCredentials: cred,
                                   issuer: _configuration.GetSection("JwtSettings:Audience").Value
   );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        
    }
}
