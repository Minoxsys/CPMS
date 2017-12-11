namespace CPMS.Notification.Manager
{
    public class PeriodBreachInfo
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public string PeriodName { get; set; }

        public string PpiNumber { get; set; }

        public int PeriodId { get; set; }

        public int DaysToBreach { get; set; }

        public string PathwayType { get; set; }

    }
}
