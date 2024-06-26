using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [EnableCors()]

    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();


        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserDetails(int userId)
        {
            var relocationDetails = await db.RelocationDetails
                .Where(rd => rd.UserId == userId)
                .FirstOrDefaultAsync();

            if (relocationDetails == null)
            {
                return NotFound("User details not found.");
            }

            var categories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Category)
                .ToListAsync();

            var userDetails = new
            {
                relocationDetails,
                categories
            };

            return Ok(userDetails);
        }
   
    }
}
