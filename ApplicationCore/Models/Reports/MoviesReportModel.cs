using System;
using System.Text.Json.Serialization;

namespace ApplicationCore.Models.Reports
{
    public class MoviesReportModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int TotalPurchases { get; set; }
        
        [JsonIgnore]
        public int MaxRows { get; set; }
    }
}