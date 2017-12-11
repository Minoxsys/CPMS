namespace CPMS.Patient.Manager
{
    public class PeriodsAndEventsBreachesCountInfo
    {
        public PeriodsBreachesCountInfo PeriodsBreachesCountInfo{ get; set; }

        public EventsBreachesCountInfo EventsBreachesCountInfoForCardiac  { get; set; }

        public EventsBreachesCountInfo EventsBreachesCountInfoForFrailElderly { get; set; }

        public EventsBreachesCountInfo EventsBreachesCountInfoForDiabetes { get; set; }
    }
}