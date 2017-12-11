namespace CPMS.Notification.Presentation
{
    public class EventBreachViewModel
    {
        public string EventCode { get; set; }

        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public string PpiNumber { get; set; }

        public int PeriodId { get; set; }

        public string Status { get; set; }

        public int DaysForStatus { get; set; }
    }
}
