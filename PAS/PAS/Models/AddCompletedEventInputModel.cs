using System.ComponentModel.DataAnnotations;
using CPMS.Domain;

namespace PAS.Models
{
    public class AddCompletedEventInputModel : AddCompletedEventViewModel
    {
        [Required]
        public EventCode SelectedEventCode { get; set; }

        [Required]
        public int SelectedClinician { get; set; }

        [Required]
        public ClockType SelectedClockType { get; set; }

        [Required]
        public string SelectedPPINumber { get; set; }

        public bool Cancer { get; set; }
    }
}