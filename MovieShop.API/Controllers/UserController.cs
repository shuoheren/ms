using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieShop.API.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPost("purchase")]
        public async Task<ActionResult> CreatePurchase([FromBody] PurchaseRequestModel purchaseRequest)
        {
            var purchasedStatus =
                await _userService.PurchaseMovie(purchaseRequest, _currentUserService.UserId.GetValueOrDefault());
            return Ok(new { purchased = purchasedStatus });
        }

        [HttpPost("favorite")]
        public async Task<ActionResult> CreateFavorite([FromBody] FavoriteRequestModel favoriteRequest)
        {
            await _userService.AddFavorite(favoriteRequest);
            return Ok();
        }

        [HttpPost("unfavorite")]
        public async Task<ActionResult> DeleteFavorite([FromBody] FavoriteRequestModel favoriteRequest)
        {
            await _userService.RemoveFavorite(favoriteRequest);
            return Ok();
        }


        [HttpGet("{id:int}/movie/{movieId}/favorite")]
        public async Task<ActionResult> IsFavoriteExists(int id, int movieId)
        {
            var favoriteExists = await _userService.FavoriteExists(id, movieId);
            return Ok(new { isFavorited = favoriteExists });
        }


        [HttpPost("review")]
        public async Task<ActionResult> CreateReview([FromBody] ReviewRequestModel reviewRequest)
        {
            await _userService.AddMovieReview(reviewRequest);
            return Ok();
        }

        [HttpPut("review")]
        public async Task<ActionResult> UpdateReview([FromBody] ReviewRequestModel reviewRequest)
        {
            await _userService.UpdateMovieReview(reviewRequest);
            return Ok();
        }


        [HttpDelete("{userId:int}/movie/{movieId:int}")]
        public async Task<ActionResult> DeleteReview(int userId, int movieId)
        {
            await _userService.DeleteMovieReview(userId, movieId);
            return NoContent();
        }

        [HttpGet("purchases")]
        public async Task<ActionResult> GetUserPurchasedMoviesAsync()
        {
            var userId = _currentUserService.UserId.GetValueOrDefault();
            var userMovies = await _userService.GetAllPurchasesForUser(userId);
            return Ok(userMovies);
        }

        [HttpGet("purchase-details/{movieId:int}")]
        public async Task<ActionResult> GetUserPurchaseDetailsAsync(int movieId)
        {
            var userId = _currentUserService.UserId.GetValueOrDefault();
            var userMovies = await _userService.GetPurchasesDetails(userId, movieId);
            return Ok(userMovies);
        }

        [HttpGet("is-movie-purchased/{movieId:int}")]
        public async Task<ActionResult> IsMoviePurchasedAsync(int movieId)
        {
            var userId = _currentUserService.UserId.GetValueOrDefault();
            var moviePurchased = await _userService.GetPurchasesDetails(userId, movieId);
            return Ok(moviePurchased == null ? new { purchased = false } : new { purchased = true });
        }

        [HttpGet("{id:int}/favorites")]
        public async Task<ActionResult> GetUserFavoriteMoviesAsync(int id)
        {
            var userMovies = await _userService.GetAllFavoritesForUser(id);
            return Ok(userMovies);
        }

        [HttpGet("{id:int}/reviews")]
        public async Task<ActionResult> GetUserReviewedMoviesAsync(int id)
        {
            var userMovies = await _userService.GetAllReviewsByUser(id);
            return Ok(userMovies);
        }
    }
}