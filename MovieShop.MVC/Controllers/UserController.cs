using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieShop.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IUserService _userService;
        private ICurrentUserService _currentUserService;
        public UserController(IUserService userService, IMovieService movieService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _movieService = movieService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> BuyMovie(int id)
        {
            var movie = await _movieService.GetMovieAsync(id);
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> BuyMovie(PurchaseRequestModel purchase)
        {
            await _userService.PurchaseMovie(purchase, _currentUserService.UserId.GetValueOrDefault());
            return Ok();
        }
    }
}