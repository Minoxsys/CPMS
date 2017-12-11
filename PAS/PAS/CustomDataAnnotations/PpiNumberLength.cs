using System.ComponentModel.DataAnnotations;

namespace PAS.CustomDataAnnotations
{
    public class PpiNumberLength : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && ((string) value).Length != 20)
            {
                return new ValidationResult("Ppi number should have 20 characters.");
            }
            return ValidationResult.Success;
        }
    }
}