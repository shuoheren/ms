﻿using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CastRepository: EfRepository<Cast>, ICastRepository
    {
        public CastRepository(MovieShopDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Cast> GetByIdAsync(int id)
        {
            var cast = await movieShopDbContext.Casts.Where(c => c.Id == id).Include(c => c.MovieCasts)
                                       .ThenInclude(c => c.Movie).FirstOrDefaultAsync();
            return cast;
        }
    }
}