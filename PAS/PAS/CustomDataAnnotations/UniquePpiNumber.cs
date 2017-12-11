using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PAS.CustomDataAnnotations
{
    public class UniquePpiNumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            using (var unitOfwork = new UnitOfWork())
            {
                if (value != null)
                {
                    var existsPpi = unitOfwork.Pathways.Any(p => p.PPINumber == value.ToString());
                    if (existsPpi)
                    {
                        return new ValidationResult("Ppi number is already assigned to another patient.");
                    }
                }
                return ValidationResult.Success;
            }
        }
    }
}