using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using ApplicationCore.Models;

namespace ApplicationCore.ServiceInterfaces
{
    public interface IMovieService
    {
        Task<PagedResultSet<MovieCardResponseModel>> GetMoviesByPagination(int pageSize = 30, int page = 1,
            string title = "");

        Task<PagedResultSet<MovieCardResponseModel>> GetMoviesByGenre(int genreId, int pageSize = 30, int pageIndex = 1);

        Task<MovieDetailsResponseModel> GetMovieAsync(int id);
        Task<IEnumerable<MovieReviewResponseModel>> GetReviewsForMovie(int id, int pageSize = 30, int pageIndex = 1);

        Task<int> GetMoviesCount(string title = "");
        Task<IEnumerable<MovieCardResponseModel>> GetTopRatedMovies();
        Task<IEnumerable<MovieCardResponseModel>> GetHighestGrossingMovies();

    }
}