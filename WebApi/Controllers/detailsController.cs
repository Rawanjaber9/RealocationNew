using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly RealocationAppContext db = new RealocationAppContext();






        //שמירת התשובות על שלושת השאלות
        //[HttpPost]
        //public async Task<IActionResult> PostRelocationDetail([FromBody] RelocationDetailDTO relocationDetailDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    System.Console.WriteLine($"DestinationCountry: {relocationDetailDto.DestinationCountry}");

        //    var relocationDetail = new RelocationDetail
        //    {
        //        UserId = relocationDetailDto.UserId,
        //        DestinationCountry = relocationDetailDto.DestinationCountry,
        //        MoveDate = relocationDetailDto.MoveDate,
        //        HasChildren = relocationDetailDto.HasChildren,
        //        CreatedAt = DateTime.Now
        //    };

        //    db.RelocationDetails.Add(relocationDetail);
        //    await db.SaveChangesAsync();

        //    // Insert selected categories into UserCategories
        //    foreach (var categoryId in relocationDetailDto.SelectedCategories)
        //    {
        //        var userCategory = new UserCategory
        //        {
        //            UserId = relocationDetailDto.UserId,
        //            CategoryId = categoryId,
        //            CreatedAt = DateTime.Now
        //        };
        //        db.UserCategories.Add(userCategory);
        //    }

        //    await db.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetRelocationDetailByUserId), new { id = relocationDetail.RelocationId }, relocationDetail);
        //}









        //אפשר למשתמש לערוך את פרטי הרילוקיישין, התשובות של  שלושת השאלות
        //[HttpPost]
        //public async Task<IActionResult> PostRelocationDetailsAndUserCategories([FromBody] CombinedInputDTO combinedInputDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    שמירת פרטי הרילוקיישן
        //   var relocationDetail = new RelocationDetail
        //   {
        //       UserId = combinedInputDto.UserId,
        //       DestinationCountry = combinedInputDto.DestinationCountry,
        //       MoveDate = combinedInputDto.MoveDate,
        //       HasChildren = combinedInputDto.HasChildren,
        //       CreatedAt = DateTime.Now
        //   };

        //    db.RelocationDetails.Add(relocationDetail);
        //    await db.SaveChangesAsync();

        //    שמירת קטגוריות המשתמש
        //   var newTasks = new List<object>();

        //    foreach (var categoryId in combinedInputDto.SelectedCategories)
        //    {
        //        בדיקה אם הקטגוריה כבר קיימת עבור המשתמש
        //       var existingCategory = await db.UserCategories
        //           .Where(uc => uc.UserId == combinedInputDto.UserId && uc.CategoryId == categoryId)
        //           .FirstOrDefaultAsync();

        //        if (existingCategory == null)
        //        {
        //            var userCategory = new UserCategory
        //            {
        //                UserId = combinedInputDto.UserId,
        //                CategoryId = categoryId,
        //                CreatedAt = DateTime.Now
        //            };
        //            db.UserCategories.Add(userCategory);
        //        }
        //    }

        //    await db.SaveChangesAsync();

        //    הוספת המשימות המומלצות למשתמש לטבלת UserTasks
        //    foreach (var categoryId in combinedInputDto.SelectedCategories)
        //    {
        //        var recommendedTasks = await db.RelocationTasks
        //            .Where(rt => rt.CategoryId == categoryId)
        //            .ToListAsync();

        //        foreach (var task in recommendedTasks)
        //        {
        //            var userTask = new UserTask
        //            {
        //                UserId = combinedInputDto.UserId,
        //                TaskId = task.TaskId,
        //                TaskName = task.RecommendedTask,
        //                TaskDescription = task.DescriptionTask,
        //                IsRecommended = true,
        //                IsDeleted = false,
        //                CreatedAt = DateTime.Now,
        //                Priority = task.PriorityId,
        //                IsBeforeMove = task.IsBeforeMove,
        //                PersonalNote = ""
        //            };
        //            db.UserTasks.Add(userTask);
        //            newTasks.Add(new { userTask.TaskId, userTask.TaskName });
        //        }
        //    }

        //    await db.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        UserId = combinedInputDto.UserId,
        //        SelectedCategories = combinedInputDto.SelectedCategories,
        //        NewTasks = newTasks
        //    });
        //}








        //מאפשר למשתמש לערוך את המענה על שלושת השאלות
        //או לבחור קטגוריות אחרות







        // הקונטרולר הזה מקבל את ה-UserId ב-URL ואת שאר הנתונים ב-Body של ה-POST request.
        //  בשלב הראשון, הוא שומר את פרטי הרילוקיישן של המשתמש.
        //   לאחר מכן, הוא שומר את הקטגוריות שנבחרו ואת המשימות המומלצות המתאימות.
        //    בסיום, הוא מבצע את חישוב תאריכי ההתחלה והסיום לכל משימה לפי תאריך המעבר ומצב המשימה (לפני או אחרי המעבר).
        //לבסוף, הוא מחזיר תשובה ל   -Client עם המידע החדש שנשמר והמשימות המעודכנות.

        [HttpPost("save-details-and-calculate/{userId}")]
        public async Task<IActionResult> PostRelocationDetailsAndCalculateTaskDates(int userId, [FromBody] CombinedInputDTO combinedInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // שלב 1: שמירת פרטי הרילוקיישן
            var relocationDetail = new RelocationDetail
            {
                UserId = combinedInputDto.UserId,
                DestinationCountry = combinedInputDto.DestinationCountry,
                MoveDate = combinedInputDto.MoveDate,
                HasChildren = combinedInputDto.HasChildren,
                CreatedAt = DateTime.Now
            };

            db.RelocationDetails.Add(relocationDetail);
            await db.SaveChangesAsync();

            // שלב 2: שמירת קטגוריות המשתמש והמשימות
            var newTasks = new List<object>();

            foreach (var categoryId in combinedInputDto.SelectedCategories)
            {
                // בדיקה אם הקטגוריה כבר קיימת עבור המשתמש
                var existingCategory = await db.UserCategories
                    .Where(uc => uc.UserId == combinedInputDto.UserId && uc.CategoryId == categoryId)
                    .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    var userCategory = new UserCategory
                    {
                        UserId = combinedInputDto.UserId,
                        CategoryId = categoryId,
                        CreatedAt = DateTime.Now
                    };
                    db.UserCategories.Add(userCategory);
                }
            }

            await db.SaveChangesAsync();

            // הוספת המשימות המומלצות למשתמש לטבלת UserTasks
            foreach (var categoryId in combinedInputDto.SelectedCategories)
            {
                var recommendedTasks = await db.RelocationTasks
                    .Where(rt => rt.CategoryId == categoryId)
                    .ToListAsync();

                foreach (var task in recommendedTasks)
                {
                    var userTask = new UserTask
                    {
                        UserId = combinedInputDto.UserId,
                        TaskId = task.TaskId,
                        TaskName = task.RecommendedTask,
                        TaskDescription = task.DescriptionTask,
                        IsRecommended = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        Priority = task.PriorityId,
                        IsBeforeMove = task.IsBeforeMove,
                        PersonalNote = "",
                        CategoryId = task.CategoryId // כאן מוסיפים את ה-CATEGORYID למשימה
                    };
                    db.UserTasks.Add(userTask);
                    newTasks.Add(new { userTask.TaskId, userTask.TaskName });
                }
            }

            await db.SaveChangesAsync();

            // שלב 3: חישוב תאריכי המשימות
            var moveDate = relocationDetail.MoveDate;

            var userTasks = await db.UserTasks
                .Where(ut => ut.UserId == userId)
                .ToListAsync();

            foreach (var userTask in userTasks)
            {
                var relocationTask = await db.RelocationTasks
                    .Where(rt => rt.TaskId == userTask.TaskId)
                    .FirstOrDefaultAsync();

                if (relocationTask == null)
                {
                    continue;
                }

                if (relocationTask.IsBeforeMove)
                {
                    // משימות שצריך לבצע לפני המעבר
                    userTask.EndDate = moveDate.AddDays(-1);
                    userTask.StartDate = userTask.EndDate.Value.AddDays((double)-relocationTask.DaysToComplete);
                }
                else
                {
                    // משימות שצריך לבצע אחרי המעבר
                    userTask.StartDate = moveDate.AddDays(1);
                    userTask.EndDate = userTask.StartDate.Value.AddDays((double)relocationTask.DaysToComplete);
                }

                db.Entry(userTask).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserId = combinedInputDto.UserId,
                SelectedCategories = combinedInputDto.SelectedCategories,
                NewTasks = newTasks,
                message = "Details saved and task dates calculated and updated successfully."
            });
        }









        [HttpPut("update-relocation-details-and-categories/{userId}")]
        public async Task<IActionResult> UpdateRelocationDetailsAndCategories(int userId, [FromBody] RelocationDetailDTO updatedDetail)
        {
            // מחיקת כל הרשומות בטבלת RelocationDetails עבור המשתמש
            var existingRelocationDetails = await db.RelocationDetails.Where(rd => rd.UserId == userId).ToListAsync();
            db.RelocationDetails.RemoveRange(existingRelocationDetails);
            await db.SaveChangesAsync();

            // יצירת רשומה חדשה עם הפרטים המעודכנים
            var relocationDetail = new RelocationDetail
            {
                UserId = userId,
                DestinationCountry = updatedDetail.DestinationCountry,
                MoveDate = updatedDetail.MoveDate.HasValue ? updatedDetail.MoveDate.Value : DateTime.Now,
                HasChildren = updatedDetail.HasChildren.HasValue ? updatedDetail.HasChildren.Value : false,
                CreatedAt = DateTime.Now
            };

            db.RelocationDetails.Add(relocationDetail);
            await db.SaveChangesAsync();

            // מחיקת כל הקטגוריות והמשימות הקיימות של המשתמש
            var existingCategories = await db.UserCategories.Where(uc => uc.UserId == userId).ToListAsync();
            db.UserCategories.RemoveRange(existingCategories);

            var existingTasks = await db.UserTasks.Where(ut => ut.UserId == userId).ToListAsync();
            db.UserTasks.RemoveRange(existingTasks);

            await db.SaveChangesAsync();

            // הוספת הקטגוריות החדשות והמשימות המתאימות לקטגוריות שנבחרו
            if (updatedDetail.SelectedCategories != null && updatedDetail.SelectedCategories.Any())
            {
                foreach (var categoryId in updatedDetail.SelectedCategories)
                {
                    // הוספת הקטגוריה לטבלת UserCategories
                    var userCategory = new UserCategory
                    {
                        UserId = userId,
                        CategoryId = categoryId,
                        CreatedAt = DateTime.Now
                    };
                    db.UserCategories.Add(userCategory);

                    // הוספת המשימות המתאימות לקטגוריה זו לטבלת UserTasks
                    var recommendedTasks = await db.RelocationTasks
                        .Where(rt => rt.CategoryId == categoryId)
                        .ToListAsync();

                    foreach (var task in recommendedTasks)
                    {
                        DateTime? startDate = null;
                        DateTime? endDate = null;

                        // חישוב תאריכי התחלה וסיום לפי תאריך הטיסה והאם המשימה מתבצעת לפני או אחרי המעבר
                        if (task.IsBeforeMove)
                        {
                            startDate = relocationDetail.MoveDate.AddDays((double)-task.DaysToComplete);
                            endDate = relocationDetail.MoveDate.AddDays(-1);
                        }
                        else
                        {
                            startDate = relocationDetail.MoveDate.AddDays(1);
                            endDate = relocationDetail.MoveDate.AddDays((double)task.DaysToComplete);
                        }

                        var userTask = new UserTask
                        {
                            UserId = userId,
                            TaskId = task.TaskId,
                            TaskName = task.RecommendedTask,
                            TaskDescription = task.DescriptionTask,
                            IsRecommended = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            StartDate = startDate,
                            EndDate = endDate,
                            Priority = task.PriorityId,
                            IsBeforeMove = task.IsBeforeMove,
                            PersonalNote = "",
                            CategoryId = task.CategoryId
                        };
                        db.UserTasks.Add(userTask);
                    }
                }

                await db.SaveChangesAsync();
            }

            // שליפת הקטגוריות המעודכנות של המשתמש
            var updatedCategories = await db.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => new
                {
                    uc.CategoryId,
                    uc.Category.CategoryName
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Relocation details, categories, and tasks updated successfully!",
                relocationDetail,
                updatedCategories
            });
        }




        //קריאה שמחזירה את מה שהמשתמש ענה בשלושת השאלות
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRelocationDetailByUserId(int userId)
        {
            var relocationDetail = await db.RelocationDetails
                .Where(rd => rd.UserId == userId)
                .FirstOrDefaultAsync();

            if (relocationDetail == null)
            {
                return NotFound("Relocation detail not found for this user.");
            }

            return Ok(relocationDetail);
        }

    }
}
