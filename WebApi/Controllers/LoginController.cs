using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();

        [HttpPost]
        
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await db.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(new { userId = user.UserId });

        }

    }
}
