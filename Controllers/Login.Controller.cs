using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PrimeraWebAPI.Models;
using PrimeraWebAPI.Services;
using ZstdSharp.Unsafe;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly TokenService _tokenService;

        [HttpGet]
        public IActionResult GetLoginPage()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "login.html");
            if (!System.IO.File.Exists(filePath)) {
                return NotFound("El archivo login.html no se encontró.");
            }
            return PhysicalFile(filePath, "text/html");
        }

        public LoginController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login) {
            List<User> users = UsersService.GetUsers();
            if (users.Any(u => u.Username == login.Username && BCrypt.Net.BCrypt.EnhancedVerify(login.Password, u.Password))) {
                User user = users.Find(u => u.Username == login.Username);
                var token = _tokenService.GenerateToken(login.Username, user.IsAdmin);
                dynamic message = new ExpandoObject();
                message.Token = token;
                if (user.LoanNews) {
                    message.LoanNews = user.LoanNewsState ? "Your loan request has been approved" : "Your loan request has been denied";
                    user.LoanNews = false;
                }
                UsersService.UpdateUser(user);
                return Ok( new {Token = token});
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
