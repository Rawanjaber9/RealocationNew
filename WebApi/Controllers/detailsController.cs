using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly RealocationAppContext db = new RealocationAppContext();

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

            return CreatedAtAction(nameof(GetRelocationDetail), new { id = relocationDetail.RelocationId }, relocationDetail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRelocationDetail(int id)
        {
            var relocationDetail = await db.RelocationDetails.FindAsync(id);

            if (relocationDetail == null)
            {
                return NotFound();
            }

            return Ok(relocationDetail);
        }
    }
}
