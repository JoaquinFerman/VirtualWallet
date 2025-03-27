using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeraWebAPI.Models;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    [Authorize (Policy = "AdminOnly")]
    public class DepositController : ControllerBase
    {
        [HttpPut]
        public IActionResult Deposit([FromQuery] int amount)
        {
            List<User> users = UsersService.GetUsers();
            User user = users.Find(u => u.Username == User.FindFirst(ClaimTypes.NameIdentifier).ToString().Split(" ")[1]);
            if (user != null) {
                if (amount < 0) {
                    return BadRequest("Amount cannot be negative");
                } else if (amount == 0) {
                    return BadRequest("Amount cannot be zero");
                } else {
                    user.Balance += amount;
                    UsersService.UpdateUser(user);
                    return Ok( new { message = "Operation successful, new balance: $" + user.Balance });
                }
            } else {
                return NotFound("User not found");
            }
        }
    }
}