using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities;
using ApplicationCore.Validations;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Models
{
    public class UserRegisterRequestModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage =
            "Password Should have minimum 8 with at least one upper, lower, number and special character")]
        public string Password { get; set; }

        [StringLength(50)] public string FirstName { get; set; }

        [StringLength(50)] public string LastName { get; set; }

        [DataType(DataType.Date)]
        [MaximumYear(1910)]
        [MinimumAge(18)]
        public DateTime DateOfBirth { get; set; }
    }

    public class UserProfileRequestModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public IFormFile File { get; set; }
    }

    public class ReviewRequestModel
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }

    public class MovieCreateRequest
    {
        public int Id { get; set; }

        [Required] [StringLength(150)] public string Title { get; set; }

        [StringLength(2084)] public string Overview { get; set; }

        [StringLength(2084)] public string Tagline { get; set; }

        [Range(0, 5000000000)]
        [RegularExpression("^(\\d{1,18})(.\\d{1})?$")]
        public decimal? Revenue { get; set; }

        [Range(0, 500000000)] public decimal? Budget { get; set; }

        [Url] public string ImdbUrl { get; set; }

        [Url] public string TmdbUrl { get; set; }

        [Required] [Url] public string PosterUrl { get; set; }

        [Required] [Url] public string BackdropUrl { get; set; }

        public string OriginalLanguage { get; set; }

        [MaximumYear(1910)]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }
        public int? RunTime { get; set; }

        [Range(.99, 49)]
        public decimal? Price { get; set; }
        public List<GenreModel> Genres { get; set; }
    }

    public class LoginRequestModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class FavoriteRequestModel
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }

    public class PurchaseRequestModel
    {
        public PurchaseRequestModel()
        {
            PurchaseDateTime = DateTime.Now;
            PurchaseNumber = Guid.NewGuid();
        }

        public Guid? PurchaseNumber { get; }
        public DateTime? PurchaseDateTime { get; }
        public int MovieId { get; set; }
    }
}