using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ApplicationCore.Models.Reports;
using ApplicationCore.RepositoryInterfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class ReportsRepository: IReportsRepository
    {
        private readonly IConfiguration _configuration;

        public ReportsRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        private SqlConnection Connection => new(_configuration.GetConnectionString("MovieShopDbConnection"));
        public IDbConnection CreateConnection()
        {
            var connection = Connection;
            if (connection.State != ConnectionState.Open && connection.State != ConnectionState.Connecting)
                connection.Open();

            return connection;
        }
        public async Task<IEnumerable<MoviesReportModel>> GetTopPurchasedMovies(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 30, int pageIndex = 1)
        {
            using (var db = CreateConnection())
            {
                var movies = await db.QueryAsync<MoviesReportModel>("usp_GetTopPurchasedMovies", new { fromDate, toDate, pageIndex, pageSize },
                    commandType: CommandType.StoredProcedure);
                return movies;
            }
        }
        
        
    }
}