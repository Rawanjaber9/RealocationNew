using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksController : ControllerBase
    {
        private readonly RealocationAppContext db;

        public UserTasksController(RealocationAppContext context)
        {
            db = context;
        }




        //אפשר למשתמש למחוק משימה ספציפית

        [HttpDelete("{userId}/task/{taskId}")]
        public async Task<IActionResult> DeleteUserTask(int userId, int taskId)
        {
            var userTask = await db.UserTasks
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TaskId == taskId && ut.IsRecommended && !ut.IsDeleted);

            if (userTask == null)
            {
                return NotFound("Task not found or already deleted.");
            }

            userTask.IsDeleted = true;
            db.Entry(userTask).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                IsDeleted = userTask.IsDeleted
            });
        }






        //הצגת המשימות המומלצות למשתמש על המסך
        [HttpGet("tasks/user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserTasks(int userId)
        {
            var userTasks = await db.UserTasks
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








        //כדי לאפשר למשתמש לעדכן את ההערה האישית לכל משימה


        [HttpPut("tasks/{userTaskId}/personalNote")]
        public async Task<IActionResult> UpdatePersonalNoteForUserTask(int userTaskId, [FromBody] PersonalNoteDTO personalNoteDto)
        {
            var userTask = await db.UserTasks.FindAsync(userTaskId);
            if (userTask == null)
            {
                return NotFound("Task not found.");
            }

            userTask.PersonalNote = personalNoteDto.PersonalNote;
            db.Entry(userTask).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                PersonalNote = userTask.PersonalNote
            });
        }



        // פעולה להוספת משימה חדשה
        [HttpPost("tasks/new")]
        public async Task<IActionResult> AddNewUserTask([FromBody] NewUserTaskDTO newUserTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userTask = new UserTask
            {
                UserId = newUserTaskDto.UserId,

                TaskName = newUserTaskDto.TaskName,
                TaskDescription = newUserTaskDto.TaskDescription,
                IsRecommended = false, // השדה מוגדר כ-FALSE
                IsDeleted = false, // השדה מוגדר כ-FALSE
                CreatedAt = DateTime.Now,
                StartDate = newUserTaskDto.StartDate,
                EndDate = newUserTaskDto.EndDate,
                Priority = newUserTaskDto.PriorityId,
                PersonalNote = newUserTaskDto.PersonalNote,
                IsNewUserTask = true // השדה מוגדר כ-TRUE
            };

            db.UserTasks.Add(userTask);
            await db.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                TaskId = 0,
                UserId = userTask.UserId,
                TaskName = userTask.TaskName,
                TaskDescription = userTask.TaskDescription,
                IsRecommended = userTask.IsRecommended,
                IsDeleted = userTask.IsDeleted,
                CreatedAt = userTask.CreatedAt,
                StartDate = userTask.StartDate,
                EndDate = userTask.EndDate,
                PriorityId = userTask.Priority,
                PersonalNote = userTask.PersonalNote,
                IsNewUserTask = userTask.IsNewUserTask
            });
        }






        // הפעלת התראה עבור משימה ספציפית לפי מספר משתמש

        [HttpPut("tasks/{userTaskId}/notification")]
        public async Task<IActionResult> UpdateTaskNotification(int userTaskId, [FromBody] UpdateNotificationDTO updateNotificationDto)
        {
            var userTask = await db.UserTasks.FindAsync(userTaskId);
            if (userTask == null)
            {
                return NotFound("Task not found.");
            }

            userTask.WantsNotification = updateNotificationDto.WantsNotification;
            db.Entry(userTask).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                WantsNotification = userTask.WantsNotification
            });
        }

    }
}
