
namespace PAS.Models
{
    public class CompletedEventViewModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }

        public string Clinician { get; set; }

        public string Comments { get; set; }

        public string PPINumber { get; set; }

        public ClockType ClockType { get; set; }

        public bool Cancer { get; set; }
    }
}