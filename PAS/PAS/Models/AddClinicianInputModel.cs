using System.ComponentModel.DataAnnotations;
using PAS.CustomDataAnnotations;

namespace PAS.Models
{
    public class AddClinicianInputModel : AddClinicianViewModel
    {
        [Required]
        public int SelectedHospital { get; set; }

        [RequiredSpecialty]
        public string SelectedSpecialty { get; set; }
    }
}