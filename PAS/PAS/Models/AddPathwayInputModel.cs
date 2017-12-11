using System.ComponentModel.DataAnnotations;

namespace PAS.Models
{
    public class AddPathwayInputModel : AddPathwayViewModel
    {
        [Required]
        public string SelectedPatientNHSNumber { get; set; }
    }
}