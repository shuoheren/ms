using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using AutoMapper;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IBlobService _blobService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICryptoService _encryptionService;
        private readonly IAsyncRepository<Favorite> _favoriteRepository;
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IAsyncRepository<Review> _reviewRepository;
        private readonly IUserRepository _userRepository;

        public UserService(ICryptoService encryptionService, IUserRepository userRepository, IMapper mapper,
            IAsyncRepository<Favorite> favoriteRepository, ICurrentUserService currentUserService,
            IMovieService movieService,
            IAsyncRepository<Review> reviewRepository, IBlobService blobService, IPurchaseRepository purchaseRepository)
        {
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _mapper = mapper;
            _favoriteRepository = favoriteRepository;
            _currentUserService = currentUserService;
            _movieService = movieService;
            _reviewRepository = reviewRepository;
            _blobService = blobService;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<UserLoginResponseModel> ValidateUser(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            var hashedPassword = _encryptionService.HashPassword(password, user.Salt);
            var isSuccess = user.HashedPassword == hashedPassword;


            var response = _mapper.Map<UserLoginResponseModel>(user);

            //var roles = userRoles.FirstOrDefault()?.Roles;
            //if (roles !=null && roles.Any())
            //    response.Roles = roles.Select(r => r.Name).ToList();
            return isSuccess ? response : null;
        }

        public async Task<UserRegisterResponseModel> CreateUser(UserRegisterRequestModel requestModel)
        {
            var dbUser = await _userRepository.GetUserByEmail(requestModel.Email);
            if (dbUser != null &&
                string.Equals(dbUser.Email, requestModel.Email, StringComparison.CurrentCultureIgnoreCase))
                throw new ConflictException("Email Already Exits");
            var salt = _encryptionService.CreateSalt();
            var hashedPassword = _encryptionService.HashPassword(requestModel.Password, salt);
            var user = new User
            {
                Email = requestModel.Email,
                Salt = salt,
                HashedPassword = hashedPassword,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };
            var createdUser = await _userRepository.AddAsync(user);

            //var response = new UserRegisterResponseModel
            //{
            //    Id = createdUser.Id, Email = createdUser.Email, FirstName = createdUser.FirstName,
            //    LastName = createdUser.LastName
            //};
            var response = _mapper.Map<UserRegisterResponseModel>(createdUser);
            return response;
        }

        public async Task<UserRegisterResponseModel> GetUserDetails(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User", id);

            var response = _mapper.Map<UserRegisterResponseModel>(user);
            return response;
        }

        public async Task<User> GetUser(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public Task<PagedResultSet<User>> GetAllUsersByPagination(int pageSize = 20, int page = 0, string lastName = "")
        {
            throw new NotImplementedException();
        }

        public async Task<Uri> UploadUserProfilePicture(UserProfileRequestModel userProfileRequestModel)
        {
            _blobService.ContainerName = "userimagescontainer";
            var result = await _blobService.UploadFileBlobAsync(userProfileRequestModel.File.OpenReadStream(),
                userProfileRequestModel.File.ContentType, userProfileRequestModel.File.FileName);
            return result;
        }

        public async Task AddFavorite(FavoriteRequestModel favoriteRequest)
        {
            // See if Movie is already Favorite.
            if (await FavoriteExists(favoriteRequest.UserId, favoriteRequest.MovieId))
                throw new ConflictException("Movie already Favorited");

            var favorite = _mapper.Map<Favorite>(favoriteRequest);
            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task RemoveFavorite(FavoriteRequestModel favoriteRequest)
        {
            var dbFavorite =
                await _favoriteRepository.ListAsync(r => r.UserId == favoriteRequest.UserId &&
                                                         r.MovieId == favoriteRequest.MovieId);
            await _favoriteRepository.DeleteAsync(dbFavorite.First());
        }

        public async Task<bool> FavoriteExists(int id, int movieId)
        {
            return await _favoriteRepository.GetExistsAsync(f => f.MovieId == movieId &&
                                                                 f.UserId == id);
        }

        public async Task<FavoriteResponseModel> GetAllFavoritesForUser(int id)
        {
            var favoriteMovies = await _favoriteRepository.ListAllWithIncludesAsync(
                p => p.UserId == _currentUserService.UserId,
                p => p.Movie);
            return _mapper.Map<FavoriteResponseModel>(favoriteMovies);
        }

        public async Task<bool> PurchaseMovie(PurchaseRequestModel purchaseRequest, int userId)
        {
            // See if Movie is already purchased.
            if (await IsMoviePurchased(purchaseRequest, userId))
                throw new ConflictException("Movie already Purchased");
            // Get Movie Price from Movie Table
            var movie = await _movieService.GetMovieAsync(purchaseRequest.MovieId);

            var purchase = new Purchase
            {
                MovieId = purchaseRequest.MovieId, PurchaseNumber = Guid.NewGuid(),
                PurchaseDateTime = DateTime.UtcNow, TotalPrice = movie.Price.GetValueOrDefault(),
                UserId = userId
            };
            //  var purchase = _mapper.Map<Purchase>(purchaseRequest);
          var createdPurchase =  await _purchaseRepository.AddAsync(purchase);
          return createdPurchase.Id >0;
        }

        public async Task<bool> IsMoviePurchased(PurchaseRequestModel purchaseRequest, int userId)
        {
            return await _purchaseRepository.GetExistsAsync(p =>
                p.UserId == userId && p.MovieId == purchaseRequest.MovieId);
        }

        public async Task<PurchaseResponseModel> GetAllPurchasesForUser(int id)
        {
            var totalPurchasesCount = await _purchaseRepository.GetCountAsync(purchase => purchase.UserId == id);
            var purchasedMovies = await _purchaseRepository.GetAllPurchasesForUser(id);

            var movies = new PurchaseResponseModel
            {
                UserId = id,
                PurchasedMovies = new List<MovieCardResponseModel>(),
                TotalMoviesPurchased = totalPurchasesCount
            };
            foreach (var purchase in purchasedMovies)
                movies.PurchasedMovies.Add(new MovieCardResponseModel
                {
                    Id = purchase.MovieId,
                    Title = purchase.Movie.Title,
                    PosterUrl = purchase.Movie.PosterUrl,
                    ReleaseDate = purchase.Movie.ReleaseDate.GetValueOrDefault()
                });

            return movies;
        }

        public async Task<PurchaseDetailsResponseModel> GetPurchasesDetails(int userId, int movieId)
        {
            var purchase = await _purchaseRepository.GetPurchaseDetails(userId, movieId);

            if (purchase == null) return null;
            var purchaseDetails = new PurchaseDetailsResponseModel
            {
                Id = purchase.Id, UserId = purchase.UserId, MovieId = purchase.MovieId,
                PurchaseNumber = purchase.PurchaseNumber,
                PurchaseDateTime = purchase.PurchaseDateTime, Title = purchase.Movie.Title,
                PosterUrl = purchase.Movie.PosterUrl, ReleaseDate = purchase.Movie.ReleaseDate.GetValueOrDefault(),
                TotalPrice = purchase.TotalPrice
            };
            return purchaseDetails;
        }

        public async Task AddMovieReview(ReviewRequestModel reviewRequest)
        {
            var review = _mapper.Map<Review>(reviewRequest);
            await _reviewRepository.AddAsync(review);
        }

        public async Task UpdateMovieReview(ReviewRequestModel reviewRequest)
        {
            var review = _mapper.Map<Review>(reviewRequest);

            await _reviewRepository.UpdateAsync(review);
        }

        public async Task DeleteMovieReview(int userId, int movieId)
        {
            var review = await _reviewRepository.ListAsync(r => r.UserId == userId && r.MovieId == movieId);
            await _reviewRepository.DeleteAsync(review.First());
        }

        public async Task<UserReviewResponseModel> GetAllReviewsByUser(int id)
        {
            var userReviews = await _userRepository.GetReviewsByUser(id);
            var userReviewModel = new UserReviewResponseModel
            {
                MovieReviews = new List<MovieReviewResponseModel>(),
                UserId = id
            };

            userReviewModel.MovieReviews = userReviews.Select(ur => new MovieReviewResponseModel
            {
                UserId = id, MovieId = ur.MovieId, Rating = ur.Rating, ReviewText = ur.ReviewText
            }).ToList();

            return userReviewModel;
        }
    }
}