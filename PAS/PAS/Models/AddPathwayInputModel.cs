using System.ComponentModel.DataAnnotations;
using CPMS.Patient.Domain;

namespace PAS.Models
{
    public class AddPathwayInputModel : AddPathwayViewModel
    {
        [Required]
        public string SelectedPatientNHSNumber { get; set; }

        [Required]
        public PathwayType Type { get; set; }
    }
}