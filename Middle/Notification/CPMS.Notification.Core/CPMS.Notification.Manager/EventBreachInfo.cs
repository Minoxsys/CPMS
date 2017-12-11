using CPMS.Patient.Domain;

namespace CPMS.Notification.Manager
{
    public class EventBreachInfo
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public EventCode EventCode { get; set; }

        public string PpiNumber { get; set; }

        public int PeriodId { get; set; }

        public EventBreachStatus? Status { get; set; }

        public int DaysForStatus { get; set; }
    }
}
