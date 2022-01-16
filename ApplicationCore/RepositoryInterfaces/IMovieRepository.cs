using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.RepositoryInterfaces
{
    public interface IMovieRepository : IAsyncRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetTopRatedMovies();
        Task<PagedResultSet<Movie>> GetMoviesByGenre(int genreId, int pageSize = 30, int page = 1);
        Task<IEnumerable<Movie>> GetHighestGrossingMovies();
        Task<IEnumerable<Review>> GetMovieReviews(int id, int pageSize = 30, int page = 1);
    }
}