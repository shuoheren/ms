using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class MovieService : IMovieService
    {
        private readonly IAsyncRepository<Favorite> _favoriteRepository;
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository, IMapper mapper,
            IAsyncRepository<Favorite> favoriteRepository)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<PagedResultSet<MovieCardResponseModel>> GetMoviesByPagination(
            int pageSize = 20, int pageIndex = 0, string title = "")
        {
            Expression<Func<Movie, bool>> filterExpression = null;
            if (!string.IsNullOrEmpty(title)) filterExpression = movie => title != null && movie.Title.Contains(title);

            var pagedMovies = await _movieRepository.GetPagedData(pageIndex, pageSize, mov => mov.OrderBy(m => m.Title),
                filterExpression);
            var movies =
                new PagedResultSet<MovieCardResponseModel>(_mapper.Map<List<MovieCardResponseModel>>(pagedMovies),
                    pagedMovies.PageIndex,
                    pageSize, pagedMovies.TotalCount);
            return movies;
        }


        public async Task<PagedResultSet<MovieCardResponseModel>> GetMoviesByGenre(int genreId, int pageSize = 30,
            int pageIndex = 1)
        {
            var pagedMovies = await _movieRepository.GetMoviesByGenre(genreId, pageSize, pageIndex);
            var movieCards = new List<MovieCardResponseModel>();
            movieCards.AddRange(pagedMovies.Data.Select(movie => new MovieCardResponseModel
            {
                Id = movie.Id, Title = movie.Title, PosterUrl = movie.PosterUrl,
                ReleaseDate = movie.ReleaseDate.GetValueOrDefault()
            }));

            return new PagedResultSet<MovieCardResponseModel>(movieCards, pageIndex, pageSize, pagedMovies.Count);
        }

        public async Task<MovieDetailsResponseModel> GetMovieAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) throw new NotFoundException("Movie", id);
            var favoritesCount = await _favoriteRepository.GetCountAsync(f => f.MovieId == id);
            var movieDetails = new MovieDetailsResponseModel
            {
                Id = movie.Id, Budget = movie.Budget, Overview = movie.Overview, Price = movie.Price,
                PosterUrl = movie.PosterUrl, Revenue = movie.Revenue,
                ReleaseDate = movie.ReleaseDate.GetValueOrDefault(), Rating = movie.Rating, Tagline = movie.Tagline,
                Title = movie.Title, RunTime = movie.RunTime,
                BackdropUrl = movie.BackdropUrl, FavoritesCount = favoritesCount, ImdbUrl = movie.ImdbUrl,
                TmdbUrl = movie.TmdbUrl
            };

            foreach (var movieGenre in movie.MovieGenres)
                movieDetails.Genres.Add(new GenreModel
                {
                    Id = movieGenre.Genre.Id, Name = movieGenre.Genre.Name
                });

            foreach (var movieCast in movie.MovieCasts)
                movieDetails.Casts.Add(new CastResponseModel
                {
                    Id = movieCast.Cast.Id, Name = movieCast.Cast.Name, Character = movieCast.Character,
                    Gender = movieCast.Cast.Gender, ProfilePath = movieCast.Cast.ProfilePath,
                    TmdbUrl = movieCast.Cast.TmdbUrl
                });

            foreach (var trailer in movie.Trailers)
                movieDetails.Trailers.Add(new TrailerResponseModel
                {
                    Id = trailer.Id, Name = trailer.Name, TrailerUrl = trailer.TrailerUrl, MovieId = trailer.MovieId
                });
            return movieDetails;
        }

        public async Task<IEnumerable<MovieReviewResponseModel>> GetReviewsForMovie(int id, int pageSize = 25,
            int page = 1)
        {
            var reviews = await _movieRepository.GetMovieReviews(id, pageSize, page);
            var reviewsMovieModel = reviews.Select(r => new MovieReviewResponseModel
            {
                MovieId = r.MovieId, Rating = r.Rating, ReviewText = r.ReviewText, UserId = r.UserId,
                Name = r.User.FirstName + " " + r.User.LastName
            });
            return reviewsMovieModel;
        }

        public async Task<int> GetMoviesCount(string title = "")
        {
            if (string.IsNullOrEmpty(title)) return await _movieRepository.GetCountAsync();
            return await _movieRepository.GetCountAsync(m => m.Title.Contains(title));
        }

        public async Task<IEnumerable<MovieCardResponseModel>> GetTopRatedMovies()
        {
            var topMovies = await _movieRepository.GetTopRatedMovies();
            var response = _mapper.Map<IEnumerable<MovieCardResponseModel>>(topMovies);
            return response;
        }

        public async Task<IEnumerable<MovieCardResponseModel>> GetHighestGrossingMovies()
        {
            var movies = await _movieRepository.GetHighestGrossingMovies();
            var response = _mapper.Map<IEnumerable<MovieCardResponseModel>>(movies);
            return response;
        }
    }
}