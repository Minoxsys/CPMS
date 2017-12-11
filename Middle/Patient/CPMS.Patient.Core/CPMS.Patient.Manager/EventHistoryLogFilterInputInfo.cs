
namespace CPMS.Patient.Manager
{
    public class EventHistoryLogFilterInputInfo
    {
        public string EventDescription { get; set; }

        public int? ImportYear { get; set; }

        public string Description { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }
    }
}
