using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Models.Reports;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using AutoMapper;

namespace Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReportsRepository _reportsRepository;

        public AdminService(IMapper mapper, IMovieRepository movieRepository, IPurchaseRepository purchaseRepository, IReportsRepository reportsRepository)
        {
            _mapper = mapper;
            _movieRepository = movieRepository;
            _purchaseRepository = purchaseRepository;
            _reportsRepository = reportsRepository;
        }

        public async Task<MovieDetailsResponseModel> CreateMovie(MovieCreateRequest movieCreateRequest)
        {
            // //if (_currentUserService.UserId != favoriteRequest.UserId)
            // //    throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to purchase");

            // // check whether the user is Admin and can create the movie claim

            // var movie = _mapper.Map<Movie>(movieCreateRequest);

            // var createdMovie = await _movieRepository.AddAsync(movie);
            //// var movieGenres = new List<MovieGenre>();
            // foreach (var genre in movieCreateRequest.Genres)
            // {
            //     var movieGenre = new MovieGenre {MovieId = createdMovie.Id, GenreId = genre.Id};
            //     await _genresRepository.AddAsync(movieGenre);
            // }

            // return _mapper.Map<MovieDetailsResponseModel>(createdMovie);
            throw new NotImplementedException();
        }

        public async Task<MovieDetailsResponseModel> UpdateMovie(MovieCreateRequest movieCreateRequest)
        {
            //var movie = _mapper.Map<Movie>(movieCreateRequest);

            //var createdMovie = await _movieRepository.UpdateAsync(movie);
            //// var movieGenres = new List<MovieGenre>();
            //foreach (var genre in movieCreateRequest.Genres)
            //{
            //    var movieGenre = new MovieGenre { MovieId = createdMovie.Id, GenreId = genre.Id };
            //    await _genresRepository.UpdateAsync(movieGenre);
            //}

            //return _mapper.Map<MovieDetailsResponseModel>(createdMovie);

            throw new NotImplementedException();
        }


        public async Task<PagedResultSet<MovieCardResponseModel>> GetAllPurchasesByMovieId(int movieId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultSet<MoviesReportModel>> GetTopPurchasedMovies(DateTime? fromDate = null,
            DateTime? toDate = null, int pageSize = 30, int pageIndex = 1)
        {
            var movies = await _reportsRepository.GetTopPurchasedMovies(fromDate, toDate, pageSize, pageIndex);

            if (movies == null) return null;
            var movieReportModel =
                new PagedResultSet<MoviesReportModel>(movies, pageIndex, pageSize, movies.FirstOrDefault().MaxRows);

            return movieReportModel;

        }

        public async Task<PagedResultSet<MovieCardResponseModel>> GetAllMoviePurchasesByPagination(int pageSize = 50,
            int page = 0)
        {
            var totalPurchases = await _purchaseRepository.GetCountAsync();
            var purchases = await _purchaseRepository.GetAllPurchases(pageSize, page);

            var data = _mapper.Map<List<MovieCardResponseModel>>(purchases);
            var purchasedMovies = new PagedResultSet<MovieCardResponseModel>(data, page, pageSize, totalPurchases);
            return purchasedMovies;
        }
    }
}