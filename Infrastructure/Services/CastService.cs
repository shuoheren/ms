using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Exceptions;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using AutoMapper;

namespace Infrastructure.Services
{
    public class CastService : ICastService
    {
        private readonly ICastRepository _castRepository;
        private readonly IMapper _mapper;

        public CastService(ICastRepository castRepository, IMapper mapper)
        {
            _castRepository = castRepository;
            _mapper = mapper;
        }

        public async Task<CastDetailsResponseModel> GetCastDetailsWithMovies(int id)
        {
            var castDetails = await _castRepository.GetByIdAsync(id);
            if (castDetails == null) throw new NotFoundException("Cast", id);

            var castDetailsModel = new CastDetailsResponseModel
            {
                Id = castDetails.Id, Gender = castDetails.Gender, ProfilePath = castDetails.ProfilePath,
                Name = castDetails.Name, TmdbUrl = castDetails.TmdbUrl,
            
                Movies = castDetails.MovieCasts.Select( mc => new MovieCardResponseModel()
                {
                    Id = mc.MovieId, Title = mc.Movie.Title, PosterUrl = mc.Movie.PosterUrl, ReleaseDate = mc.Movie.ReleaseDate.GetValueOrDefault()
                    
                })
            };

         
            return castDetailsModel;
        }
    }
}