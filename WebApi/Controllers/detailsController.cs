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




        [HttpPost]
        public async Task<IActionResult> PostRelocationDetailsAndUserCategories([FromBody] CombinedInputDTO combinedInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // שמירת פרטי הרילוקיישן
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

            // שמירת קטגוריות המשתמש
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
                        PersonalNote = ""
                    };
                    db.UserTasks.Add(userTask);
                    newTasks.Add(new { userTask.TaskId, userTask.TaskName });
                }
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                UserId = combinedInputDto.UserId,
                SelectedCategories = combinedInputDto.SelectedCategories,
                NewTasks = newTasks
            });
        }














        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateRelocationDetail(int userId, [FromBody] RelocationDetail updatedDetail)
        {
            // מצא את הרשומה עבור המשתמש
            var relocationDetail = await db.RelocationDetails
                .FirstOrDefaultAsync(rd => rd.UserId == userId);

            if (relocationDetail == null)
            {
                return NotFound("Relocation detail not found for this user.");
            }

            // עדכון פרטים
            relocationDetail.DestinationCountry = updatedDetail.DestinationCountry;
            relocationDetail.MoveDate = updatedDetail.MoveDate;
            relocationDetail.HasChildren = updatedDetail.HasChildren;
            relocationDetail.CreatedAt = DateTime.Now; // עדכון זמן העריכה

            // שמירת השינויים
            db.Entry(relocationDetail).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Relocation details updated successfully!",
                relocationDetail
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
