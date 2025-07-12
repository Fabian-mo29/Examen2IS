using Microsoft.AspNetCore.Mvc;
using back_end.Application;
using back_end.Domain;

namespace back_end.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SodaController : ControllerBase
    {
        private readonly ISodaQuery _sodaQuery;

        public SodaController(ISodaQuery sodaQuery)
        {
            _sodaQuery = sodaQuery;
        }
        
        [HttpGet]
        public ActionResult<List<Soda>> GetAll()
        {
            try
            {
                var sodas = _sodaQuery.GetAll();
                return Ok(sodas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Error retrieving sodas",
                });
            }
        }
    }
}