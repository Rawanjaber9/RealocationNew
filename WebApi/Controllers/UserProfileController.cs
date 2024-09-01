using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly RealocationAppContext db;

        public UserProfileController(RealocationAppContext context)
        {
            db = context;
        }

        [HttpPost("upload-profile-picture/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            byte[] pictureData;
            using (var ms = new MemoryStream())
            {
                await profilePicture.CopyToAsync(ms);
                pictureData = ms.ToArray();
            }

            var userProfilePicture = new UserProfilePicture
            {
                UserId = userId,
                ProfilePicture = pictureData,
                CreatedAt = DateTime.Now
            };

            db.UserProfilePictures.Add(userProfilePicture);
            await db.SaveChangesAsync();

            return Ok(new { message = "Profile picture uploaded successfully!" });
        }





        [HttpGet("get-profile-picture/{userId}")]
        public async Task<IActionResult> GetProfilePicture(int userId)
        {
            var userProfilePicture = await db.UserProfilePictures
                .FirstOrDefaultAsync(upp => upp.UserId == userId);

            if (userProfilePicture == null)
            {
                return NotFound("Profile picture not found.");
            }

            return File(userProfilePicture.ProfilePicture, "image/jpeg");
        }



    }
}
