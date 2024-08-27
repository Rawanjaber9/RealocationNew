using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTaskController : ControllerBase
    {
        private readonly RealocationAppContext db = new RealocationAppContext();


        //האפשרות לערוך משימה אחת ספציפית
        //ניתן לערוך חלק מהמשימה ולא את כולה
        //כלומר ניתן לערוך רק את רמת הדחיפות 
        //או את רמת הדחיפות ותיאור המשימה 
        //אך גם יהיה אפשר לערוך את כל פרטי המשימה

        [HttpPut("tasks/update/{userTaskId}")]
        public async Task<IActionResult> UpdateUserTask(int userTaskId, [FromBody] UpdateTaskDTO updateTaskDto)
        {
            var userTask = await db.UserTasks.FindAsync(userTaskId);
            if (userTask == null)
            {
                return NotFound("Task not found.");
            }

            // בדיקה ועדכון חלקי של השדות
            if (!string.IsNullOrEmpty(updateTaskDto.TaskName))
            {
                userTask.TaskName = updateTaskDto.TaskName;
            }

            if (!string.IsNullOrEmpty(updateTaskDto.TaskDescription))
            {
                userTask.TaskDescription = updateTaskDto.TaskDescription;
            }

            if (updateTaskDto.PriorityId != null)
            {
                userTask.Priority = updateTaskDto.PriorityId;
            }

            if (updateTaskDto.StartDate != null)
            {
                userTask.StartDate = updateTaskDto.StartDate;
            }

            if (updateTaskDto.EndDate != null)
            {
                userTask.EndDate = updateTaskDto.EndDate;
            }

            if (!string.IsNullOrEmpty(updateTaskDto.PersonalNote))
            {
                userTask.PersonalNote = updateTaskDto.PersonalNote;
            }

            db.Entry(userTask).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(new
            {
                UserTaskId = userTask.UserTaskId,
                TaskName = userTask.TaskName,
                TaskDescription = userTask.TaskDescription,
                Priority = userTask.Priority,
                StartDate = userTask.StartDate,
                EndDate = userTask.EndDate,
                PersonalNote = userTask.PersonalNote
            });
        }

    }
}
