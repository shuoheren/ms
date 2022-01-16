using System.Threading.Tasks;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CastController : ControllerBase
    {
        private readonly ICastService _castService;

        public CastController(ICastService castService)
        {
            _castService = castService;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCastDetails(int id)
        {
            var cast = await _castService.GetCastDetailsWithMovies(id);
            return Ok(cast);
        }
    }
}
