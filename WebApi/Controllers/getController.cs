using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    //The Controller take the countries names from the Countries table in DataBase//


    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class getController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            return await db.Countries.ToListAsync();
        }
    }
} 
