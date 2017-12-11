using System;

namespace CPMS.Patient.Domain
{
    public class PlannedEvent
    {
        public int Id { get; set; }

        public EventCode Code { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public Event Event { get; set; }

        public DateTime? DateReferenceForTarget { get; set; }

        public DateTime? BreachDate
        {
            get
            {
                if (TargetNumberOfDays != null && DateReferenceForTarget != null)
                {
                    return ((DateTime) DateReferenceForTarget).AddDays((int)TargetNumberOfDays + Event.Period.GetNumberOfPausedDays((DateTime) DateReferenceForTarget, Event.EventDate));
                }

                return null;
            }
        }

        public EventBreachStatus? BreachStatusAt(DateTime dateTime)
        {
            if (GetDaysToBreachAt(dateTime) == 0)
            {
                return EventBreachStatus.Breach;
            }
            if (GetDaysToBreachAt(dateTime) > 0 && GetDaysToBreachAt(dateTime) <= 3)
            {
                return EventBreachStatus.AboutToBreach;
            }
            if (GetDaysToBreachAt(dateTime) < 0)
            {
                return EventBreachStatus.Breached;
            }
            return null;
        }

        public int? GetDaysToBreachAt(DateTime dateTime)
        {
            return BreachDate == null ? (int?)null : ((DateTime)BreachDate - dateTime.AddDays(-Event.Period.GetNumberOfPausedDays(Event.EventDate, dateTime))).Days;
        }
    }
}
