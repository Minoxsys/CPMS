using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PAS.CustomDataAnnotations;

namespace PAS.Models
{
    public class AddPathwayViewModel
    {
        [Required, MaxLength(20)]
        [RegularExpression(@"^[0-9A-Za-z]+$", ErrorMessage = "Special characters should not be entered.")]
        [UniquePpiNumber]
        [PpiNumberLength]
        public string PPINumber { get; set; }

        [Required, MaxLength(20)]
        public string OrganizationCode { get; set; }

        public IEnumerable<LitePatientViewModel> Patients { get; set; }

        public IEnumerable<PathwayViewModel> AllPathways { get; set; }
    }
}