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
        [HttpPost]
        public async Task<IActionResult> PostRelocationDetail([FromBody] RelocationDetailDTO relocationDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            System.Console.WriteLine($"DestinationCountry: {relocationDetailDto.DestinationCountry}");

            var relocationDetail = new RelocationDetail
            {
                UserId = relocationDetailDto.UserId,
                DestinationCountry = relocationDetailDto.DestinationCountry,
                MoveDate = relocationDetailDto.MoveDate,
                HasChildren = relocationDetailDto.HasChildren,
                CreatedAt = DateTime.Now
            };

            db.RelocationDetails.Add(relocationDetail);
            await db.SaveChangesAsync();

            // Insert selected categories into UserCategories
            foreach (var categoryId in relocationDetailDto.SelectedCategories)
            {
                var userCategory = new UserCategory
                {
                    UserId = relocationDetailDto.UserId,
                    CategoryId = categoryId,
                    CreatedAt = DateTime.Now
                };
                db.UserCategories.Add(userCategory);
            }

            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRelocationDetailByUserId), new { id = relocationDetail.RelocationId }, relocationDetail);
        }









        //אפשר למשתמש לערוך את פרטי הרילוקיישין, התשובות של  שלושת השאלות
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
