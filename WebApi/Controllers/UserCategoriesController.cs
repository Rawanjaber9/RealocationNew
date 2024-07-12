using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCategoriesController : ControllerBase
    {
        private readonly RealocationAppContext db = new RealocationAppContext();

        [HttpPost]
        public async Task<IActionResult> PostUserCategories([FromBody] UserCategoriesInputDTO userCategoriesDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var categoryId in userCategoriesDto.SelectedCategories)
            {
                var userCategory = new UserCategory
                {
                    UserId = userCategoriesDto.UserId,
                    CategoryId = categoryId,
                    CreatedAt = DateTime.Now
                };
                db.UserCategories.Add(userCategory);
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserId = userCategoriesDto.UserId,
                SelectedCategories = userCategoriesDto.SelectedCategories
            });
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetUserCategories(int userId)
        {
            var categories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Category)
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found for this user.");
            }

            return Ok(categories);
        }

        [HttpGet("tasks/user/{userId}/{isBeforeMove}")]
        public async Task<ActionResult<IEnumerable<RelocationTask>>> GetTasksByUserAndMoveStatus(int userId, bool isBeforeMove)
        {
            // מציאת הקטגוריות שהמשתמש בחר
            var userCategories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.CategoryId)
                .ToListAsync();

            // מציאת המשימות לפי הקטגוריות והמצב מעבר
            var tasks = await db.RelocationTasks
                .Where(rt => userCategories.Contains(rt.CategoryId) && rt.IsBeforeMove == isBeforeMove)
                .ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks found for the user's categories and move status.");
            }

            return Ok(tasks);
        }

    }
}
