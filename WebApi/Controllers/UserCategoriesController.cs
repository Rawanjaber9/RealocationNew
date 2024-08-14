using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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


            var newTasks = new List<object>();


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

            // הוספת המשימות המומלצות למשתמש לטבלת UserTasks
            foreach (var categoryId in userCategoriesDto.SelectedCategories)
            {
                var recommendedTasks = await db.RelocationTasks
                    .Where(rt => rt.CategoryId == categoryId)
                    .ToListAsync();

                foreach (var task in recommendedTasks)
                {
                    var userTask = new UserTask
                    {
                        UserId = userCategoriesDto.UserId,
                        TaskId = task.TaskId,
                        TaskName = task.RecommendedTask,
                        TaskDescription = task.DescriptionTask,
                        IsRecommended = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        Priority = task.PriorityId,
                        PersonalNote = ""
                    };
                    db.UserTasks.Add(userTask);
                    newTasks.Add(new { userTask.TaskId, userTask.TaskName });

                }
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserId = userCategoriesDto.UserId,
                SelectedCategories = userCategoriesDto.SelectedCategories,
                NewTasks = newTasks


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




        //פקודה שמחזירה את הקטגוריות שהמשתמש בחר יחד עם המשימות של כל קטגוריה
        [HttpGet("tasks/user/{userId}/{isBeforeMove}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTasksByUserAndMoveStatus(int userId, bool isBeforeMove)
        {
            // מציאת הקטגוריות שהמשתמש בחר
            var userCategories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.CategoryId)
                .ToListAsync();

            // מציאת המשימות לפי הקטגוריות והמצב מעבר
            var tasks = await db.RelocationTasks
                .Where(rt => userCategories.Contains(rt.CategoryId) && rt.IsBeforeMove == isBeforeMove)
                .Select(rt => new
                {
                    rt.CategoryId,
                    rt.TaskId,
                    rt.RecommendedTask,
                    rt.DescriptionTask
                })
                .ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks found for the user's categories and move status.");
            }

            return Ok(tasks);
        }








    }
}
