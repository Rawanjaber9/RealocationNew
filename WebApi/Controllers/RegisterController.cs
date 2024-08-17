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
                user.CreatedAt = DateTime.UtcNow;
                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Ok(new { message = "Registration successful!", user.UserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPut("accept-terms/{userId}")]
        public async Task<IActionResult> AcceptTerms(int userId, [FromBody] bool hasAcceptedTerms)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.HasAcceptedTerms = hasAcceptedTerms;
            db.Entry(user).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return Ok(new { message = "Terms accepted status updated successfully!", user.UserId, user.HasAcceptedTerms });
        }



        //פקודה המחזירה את פרטי המשתמש

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserDetails(int userId)
        {
            var user = await db.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

    }
}
