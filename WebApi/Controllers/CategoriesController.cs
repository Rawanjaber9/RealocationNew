using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RealocationAppContext db = new RealocationAppContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await db.Categories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }

            return Ok(categories);
        }
    }
}
