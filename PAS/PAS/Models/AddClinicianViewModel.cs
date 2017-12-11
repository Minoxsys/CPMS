using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.Models
{
    public class AddClinicianViewModel
    {
        [Required, MaxLength(30)]
        public string Name { get; set; }

        public IEnumerable<LiteSpecialtyViewModel> Specialties { get; set; }

        public IEnumerable<LiteHospitalViewModel> Hospitals { get; set; }

        public IEnumerable<ClinicianViewModel> AllClinicians { get; set; }
    }
}