using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Username, Password and Email are required.");
            }

            if (await db.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.CreatedAt = DateTime.UtcNow; // Ensure CreatedAt is set
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Ok(new { message = "Registration successful!", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
