using System;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieShop.API.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("movie")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreateRequest movieCreateRequest)
        {
            var createdMovie = await _adminService.CreateMovie(movieCreateRequest);
            return CreatedAtRoute("GetMovie", new { id = createdMovie.Id }, createdMovie);
        }

        [HttpPut("movie")]
        public async Task<IActionResult> UpdateMovie([FromBody] MovieCreateRequest movieCreateRequest)
        {
            var createdMovie = await _adminService.UpdateMovie(movieCreateRequest);
            return Ok(createdMovie);
        }

       [HttpGet("reports/toppurchases")]
        public async Task<IActionResult> GetTopPurchases([FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageSize = 30, [FromQuery] int pageIndex = 1)
        {
            var movies = await _adminService.GetTopPurchasedMovies(fromDate, toDate, pageSize,pageIndex );
            return Ok(movies);
        }
    }
}