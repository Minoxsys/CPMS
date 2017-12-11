namespace CPMS.Report.Presentation
{
    public class EventPerformanceViewModel
    {
        public string EventDescription { get; set; }

        public int BreachedCompletedEventsNumber { get; set; }

        public int BreachedEventMilestonesNumber { get; set; }

        public int TotalCompletedEventsNumber { get; set; }

        public int TotalEventMilestonesNumber { get; set; }
    }
}
