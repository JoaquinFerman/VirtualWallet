using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    [Authorize]
    public class TokenController : ControllerBase {
        [HttpGet]
        [Authorize]
        public IActionResult Check() {
            return Ok();
        }
    }
}