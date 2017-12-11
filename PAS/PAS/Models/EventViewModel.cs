using CPMS.Patient.Domain;

namespace PAS.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }

        public EventCode Code { get; set; }

        public string Date { get; set; }

        public string Clinician { get; set; }

        public string Comments { get; set; }

        public string PPINumber { get; set; }

        public ClockType ClockType { get; set; }

        public bool Cancer { get; set; }
    }
}