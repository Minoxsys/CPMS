using CPMS.Domain;

namespace CPMS.Notification.Manager
{
    public class EventBreachInfo
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public string EventDescription { get; set; }

        public string PpiNumber { get; set; }

        public int PeriodId { get; set; }

        public EventBreachStatus? Status { get; set; }

        public int DaysForStatus { get; set; }
    }
}
