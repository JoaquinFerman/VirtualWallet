using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PrimeraWebAPI.Models;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    [Authorize]
    public class BalanceController : ControllerBase
    {
        [HttpGet]
        public IActionResult Balance()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = UsersService.GetUsers().Find(u => u.Username == username);
            if (user == null) {
                return NotFound("User not found");
            } else {
                int balance = user.Balance;
                return Ok( new { Balance = balance });
            }
        }
    }
}