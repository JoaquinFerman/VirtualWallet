using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeraWebAPI.Models;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    public class AdminController : ControllerBase {

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetUsers() {
            var users = UsersService.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetUserByID([FromRoute]Guid id) {
            var users = UsersService.GetUsers();

            if (users.Any(u => u.Id == id) == false) {
                return NotFound("Usuario no encontrado");
            }

            return Ok(users.FirstOrDefault(u => u.Id == id));
        }

        public class AddUserModel(){
            public string Username { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddUser([FromBody] AddUserModel user) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (new List<string> { user.Username, user.Name, user.Surname, user.Password }.Any(f => f == default)){
                return BadRequest("Faltan campos obligatorios");
            } 
            var users = UsersService.GetUsers();

            if (users.Any(u => u.Username == user.Username)) {
                return BadRequest("Nombre de usuario no disponible");
            }
            User newUser = new User(user.Name, user.Surname, user.Username, user.Password);
            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, workFactor: 12);
            UsersService.AddUser(newUser);

            return CreatedAtAction(nameof(GetUserByID), new { newUser.Id }, newUser);
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUser([FromBody] User user) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (new List<Object> { user.Username, user.Name, user.Surname, user.Password }.Any(f => f == default)){
                return BadRequest("Faltan campos obligatorios");
            }            
            if (UsersService.SearchUser(user.Username) != null) {
                return NotFound("Usuario no encontrado");
            } else {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await UsersService.UpdateUser(user);
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUser(Guid id) {
            var users = UsersService.GetUsers();

            if (id == Guid.NewGuid()) {
                return BadRequest("Id no valido");
            }
            else if (users.Any(u => u.Id == id) == false) {
                return NotFound("Usuario no encontrado");
            }

            UsersService.DeleteUser(id);

            return Ok();
        }
    }
}
