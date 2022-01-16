using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using AutoMapper;

namespace Infrastructure.Helpers
{
    public class MovieShopMappingProfile : Profile
    {
        public MovieShopMappingProfile()
        {
            CreateMap<Movie, MovieCardResponseModel>();
            CreateMap<Cast, CastDetailsResponseModel>()
                .ForMember(c => c.Movies, opt => opt.MapFrom(src => GetMoviesForCast(src.MovieCasts)));

            CreateMap<Movie, MovieDetailsResponseModel>()
                .ForMember(md => md.Casts, opt => opt.MapFrom(src => GetCasts(src.MovieCasts)));
            //  .ForMember(md => md.Genres, opt => opt.MapFrom(src => GetMovieGenres(src.MovieGenres)));

            CreateMap<User, UserRegisterResponseModel>();
            
            CreateMap<IEnumerable<Favorite>, FavoriteResponseModel>()
                .ForMember(p => p.FavoriteMovies, opt => opt.MapFrom(src => GetFavoriteMovies(src)))
                .ForMember(p => p.UserId, opt => opt.MapFrom(src => src.FirstOrDefault().UserId));

            CreateMap<IEnumerable<Review>, UserReviewResponseModel>()
                .ForMember(r => r.MovieReviews, opt => opt.MapFrom(src => GetUserReviewedMovies(src)))
                .ForMember(r => r.UserId, opt => opt.MapFrom(src => src.FirstOrDefault().UserId));

            CreateMap<Review, MovieReviewResponseModel>()
                .ForMember(r => r.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            CreateMap<Purchase, MovieCardResponseModel>().ForMember(p => p.Id, opt => opt.MapFrom(src => src.Movie.Id))
                .ForMember(p => p.Title, opt => opt.MapFrom(src => src.Movie.Title))
                .ForMember(p => p.PosterUrl, opt => opt.MapFrom(src => src.Movie.PosterUrl));

            CreateMap<User, UserLoginResponseModel>();
            CreateMap<Role, RoleModel>();
            CreateMap<Genre, GenreModel>().ReverseMap();

            CreateMap<MovieCreateRequest, Movie>();
            //.ForMember( m => m.MovieGenres, opt => opt.MapFrom( src => GetMovieGenres(src.Genres)));

            // Request Models to Db Entities Mappings
            CreateMap<PurchaseRequestModel, Purchase>();
            CreateMap<FavoriteRequestModel, Favorite>();
            CreateMap<ReviewRequestModel, Review>();

        }

        //private List<Genre> GetMovieGenres(IEnumerable<MovieGenre> srcGenres)
        //{
        //    var movieGenres = new List<Genre>();
        //    foreach (var genre in srcGenres)
        //    {
        //        movieGenres.Add(new Genre { Id = genre.GenreId, Name = genre.Genre.Name });
        //    }

        //    return movieGenres;
        //}

        private List<MovieReviewResponseModel> GetUserReviewedMovies(IEnumerable<Review> reviews)
        {
            var reviewResponse = new UserReviewResponseModel {MovieReviews = new List<MovieReviewResponseModel>()};

            foreach (var review in reviews)
                reviewResponse.MovieReviews.Add(new MovieReviewResponseModel
                {
                    MovieId = review.MovieId,
                    Rating = review.Rating,
                    UserId = review.UserId,
                    ReviewText = review.ReviewText
                });

            return reviewResponse.MovieReviews;
        }

        private List<FavoriteResponseModel.FavoriteMovieResponseModel> GetFavoriteMovies(
            IEnumerable<Favorite> favorites)
        {
            var favoriteResponse = new FavoriteResponseModel
            {
                FavoriteMovies = new List<FavoriteResponseModel.FavoriteMovieResponseModel>()
            };
            foreach (var favorite in favorites)
                favoriteResponse.FavoriteMovies.Add(new FavoriteResponseModel.FavoriteMovieResponseModel
                {
                    PosterUrl = favorite.Movie.PosterUrl,
                    Id = favorite.MovieId,
                    Title = favorite.Movie.Title
                });

            return favoriteResponse.FavoriteMovies;
        }

        private List<MovieCardResponseModel> GetMoviesForCast(IEnumerable<MovieCast> srcMovieCasts)
        {
            var castMovies = new List<MovieCardResponseModel>();
            foreach (var movie in srcMovieCasts)
                castMovies.Add(new MovieCardResponseModel
                {
                    Id = movie.MovieId,
                    PosterUrl = movie.Movie.PosterUrl,
                    Title = movie.Movie.Title,
                    ReleaseDate = movie.Movie.ReleaseDate.GetValueOrDefault()
                });

            return castMovies;
        }

        private static List<CastResponseModel> GetCasts(IEnumerable<MovieCast> srcMovieCasts)
        {
            var movieCast = new List<CastResponseModel>();
            foreach (var cast in srcMovieCasts)
                movieCast.Add(new CastResponseModel
                {
                    Id = cast.CastId,
                    Gender = cast.Cast.Gender,
                    Name = cast.Cast.Name,
                    ProfilePath = cast.Cast.ProfilePath,
                    TmdbUrl = cast.Cast.TmdbUrl,
                    Character = cast.Character
                });

            return movieCast;
        }
    }
}