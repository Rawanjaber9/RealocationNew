using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksController : ControllerBase
    {
        private readonly RealocationAppContext _context;

        public UserTasksController(RealocationAppContext context)
        {
            _context = context;
        }

        [HttpDelete("{userId}/task/{taskId}")]
        public async Task<IActionResult> DeleteUserTask(int userId, int taskId)
        {
            var userTask = await _context.UserTasks
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TaskId == taskId && ut.IsRecommended && !ut.IsDeleted);

            if (userTask == null)
            {
                return NotFound("Task not found or already deleted.");
            }

            userTask.IsDeleted = true;
            _context.Entry(userTask).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                IsDeleted = userTask.IsDeleted
            });
        }







        [HttpGet("tasks/user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserTasks(int userId)
        {
            var userTasks = await _context.UserTasks
                .Where(ut => ut.UserId == userId && !ut.IsDeleted)
                .Select(ut => new
                {
                    ut.UserTaskId,
                    ut.TaskName,
                    ut.TaskDescription,
                    ut.IsRecommended,
                    ut.IsDeleted,
                    ut.CreatedAt
                })
                .ToListAsync();

            if (userTasks == null || !userTasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            return Ok(userTasks);
        }
    }
}
