using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCountriesController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();
        //מביא את שמות המדינות
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            return await db.Countries.ToListAsync();
        }
    }
}
