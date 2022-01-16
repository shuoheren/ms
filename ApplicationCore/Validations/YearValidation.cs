using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Validations
{
    public class MaximumYearAttribute : ValidationAttribute
    {
        public MaximumYearAttribute(int year)
        {
            Year = year;
        }

        public int Year { get; }

        public string GetErrorMessage()
        {
            return $"year should be no less than {Year}.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var year = ((DateTime?) value)?.Year;

            if (year < Year) return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }
    }
}