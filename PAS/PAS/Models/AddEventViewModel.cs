using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.Models
{
    public class AddEventViewModel
    {
        public IEnumerable<EventViewModel> AllEvents { get; set; }

        public IEnumerable<LiteClinicianViewModel> Clinicians { get; set; }

        public IEnumerable<string> Pathways { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public string Comments { get; set; }

        public IEnumerable<string> ClockTypes { get; set; }
    }
}