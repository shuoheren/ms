using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Validations
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        public MinimumAgeAttribute(int minimumAge)
        {
            MinimumAge = minimumAge;
        }

        public int MinimumAge { get; }

        public string GetErrorMessage()
        {
            return $"You must be at least {MinimumAge} years to register";
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var dateOfBirth = (DateTime) value;
            var age = DateTime.Now.Year - dateOfBirth.Year;
            return age < MinimumAge ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }
    }
}