using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Models.Reports;

namespace ApplicationCore.RepositoryInterfaces
{
    public interface IReportsRepository
    {
        Task<IEnumerable<MoviesReportModel>> GetTopPurchasedMovies(DateTime? fromDate = null, DateTime? toDate = null,
            int pageSize = 30, int pageIndex = 1);
    }
}