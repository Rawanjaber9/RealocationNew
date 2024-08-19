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
            // בדוק אם למשתמש יש ילדים
            var hasChildren = await db.RelocationDetails
                .Where(rd => rd.UserId == userId)
                .Select(rd => rd.HasChildren)
                .FirstOrDefaultAsync();

            // מצא את הקטגוריות שהמשתמש בחר
            var selectedCategories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.CategoryId)
                .ToListAsync();

            // שלוף את כל פרטי המשימות המומלצות מהקטגוריות שהמשתמש בחר
            var recommendedTasks = await db.RelocationTasks
                .Where(rt => selectedCategories.Contains(rt.CategoryId))
                .Select(rt => new
                {
                    rt.TaskId,
                    rt.CategoryId,
                    rt.RecommendedTask,
                    rt.DescriptionTask,
                    rt.IsBeforeMove,
                    rt.DaysToComplete,
                    rt.PriorityId,
                    rt.IsForParents
                })
                .ToListAsync();

            // אם למשתמש יש ילדים, שלוף גם את כל פרטי המשימות שמיועדות להורים
            if (hasChildren)
            {
                var parentTasks = await db.RelocationTasks
                    .Where(rt => rt.IsForParents)
                    .Select(rt => new
                    {
                        rt.TaskId,
                        rt.CategoryId,
                        rt.RecommendedTask,
                        rt.DescriptionTask,
                        rt.IsBeforeMove,
                        rt.DaysToComplete,
                        rt.PriorityId,
                        rt.IsForParents
                    })
                    .ToListAsync();

                recommendedTasks.AddRange(parentTasks);
            }

            if (!recommendedTasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            return Ok(recommendedTasks);
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




        //קונטרולר שמחזיר את המשימות אחרי הסינון
        [HttpGet("tasks/user/{userId}/final")]
        public async Task<ActionResult<IEnumerable<object>>> GetFinalUserTasks(int userId)
        {
            var finalUserTasks = await db.UserTasks
                .Where(ut => ut.UserId == userId && !ut.IsDeleted)
                .Select(ut => new
                {
                    ut.UserTaskId,
                    ut.TaskName,
                    ut.TaskDescription,
                    ut.IsRecommended,
                    ut.CreatedAt,
                    ut.StartDate,
                    ut.EndDate,
                    ut.Priority,
                    ut.PersonalNote,
                    ut.WantsNotification,
                    ut.IsNewUserTask,
                    ut.IsDeleted,
                })
                .ToListAsync();

            if (finalUserTasks == null || !finalUserTasks.Any())
            {
                return NotFound("No tasks found for this user.");
            }

            return Ok(finalUserTasks);
        }





        //חישוב ימי ביצוע המשימה 
        [HttpPost("calculate-tasks-dates/{userId}")]
        public async Task<IActionResult> CalculateTasksDates(int userId)
        {
            // שלב 1: שליפת תאריך המעבר של המשתמש מטבלת RelocationDetail
            var relocationDetail = await db.RelocationDetails
                .Where(rd => rd.UserId == userId)
                .FirstOrDefaultAsync();

            if (relocationDetail == null)
            {
                return NotFound("Relocation details not found for the user.");
            }

            var moveDate = relocationDetail.MoveDate;

            // שלב 2: שליפת כל המשימות עבור המשתמש מטבלת UserTasks
            var userTasks = await db.UserTasks
                .Where(ut => ut.UserId == userId)
                .ToListAsync();

            foreach (var userTask in userTasks)
            {
                // שלב 3: שליפת פרטי המשימה מטבלת RelocationTasks לפי TaskId
                var relocationTask = await db.RelocationTasks
                    .Where(rt => rt.TaskId == userTask.TaskId)
                    .FirstOrDefaultAsync();

                if (relocationTask == null)
                {
                    continue; // אם לא נמצאה משימה תמשיך למשימה הבאה
                }

                // שלב 4: חישוב תאריכי התחלה וסיום בהתאם לכמות הימים ולמצב המעבר
                if (relocationTask.IsBeforeMove)
                {
                    // אם המשימה צריכה להתבצע אחרי תאריך המעבר
                    userTask.StartDate = moveDate.AddDays(1); // התאריך הבא אחרי תאריך המעבר
                    userTask.EndDate = userTask.StartDate.Value.AddDays((double)relocationTask.DaysToComplete);
                }
                else
                {
                    // אם המשימה צריכה להתבצע לפני תאריך המעבר
                    userTask.EndDate = moveDate.AddDays(-1); // התאריך יום לפני המעבר
                    userTask.StartDate = userTask.EndDate.Value.AddDays((double)-relocationTask.DaysToComplete);
                }

                // עדכון המשימה עם תאריכי ההתחלה והסיום המחושבים
                db.Entry(userTask).State = EntityState.Modified;
            }

            // שלב 5: שמירת השינויים בבסיס הנתונים
            await db.SaveChangesAsync();

            return Ok(new { message = "Task dates calculated and updated successfully." });
        }

    }
}
