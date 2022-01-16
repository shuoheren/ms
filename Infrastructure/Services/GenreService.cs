using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services
{
    public class GenreService : IGenreService
    {
        private readonly IAsyncRepository<Genre> _genreRepository;
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromDays(30);
        private static readonly string _genresKey = "genres";
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public GenreService(IAsyncRepository<Genre> genreRepository, IMemoryCache cache, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreModel>> GetAllGenres()
        {
            var genres = await _cache.GetOrCreateAsync(_genresKey, Factory);
            return genres.OrderBy(g => g.Name);
        }

        private async Task<IEnumerable<GenreModel>> Factory(ICacheEntry entry)
        {
            entry.SlidingExpiration = DefaultCacheDuration;
            var dbGenres = await _genreRepository.ListAllAsync();
            return _mapper.Map<IEnumerable<GenreModel>>(dbGenres);
        }
    }
}