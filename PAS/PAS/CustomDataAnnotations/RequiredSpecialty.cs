using System.ComponentModel.DataAnnotations;

namespace PAS.CustomDataAnnotations
{
    public class RequiredSpecialty : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value == null || value.ToString() == "0" ? new ValidationResult("Selecting a specialty is required.") : ValidationResult.Success;
        }
    }
}