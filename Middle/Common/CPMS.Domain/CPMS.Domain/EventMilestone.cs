using System;

namespace CPMS.Domain
{
    public class EventMilestone
    {
        public int Id { get; set; }

        public EventName Name { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public CompletedEvent CompletedEvent { get; set; }

        public DateTime? DateReferenceForTarget { get; set; }

        public DateTime? BreachDate
        {
            get
            {
                if (TargetNumberOfDays != null && DateReferenceForTarget != null)
                {
                    return ((DateTime)DateReferenceForTarget).AddDays((int)TargetNumberOfDays +
                                  CompletedEvent.Period.GetNumberOfPausedDays((DateTime)DateReferenceForTarget, CompletedEvent.EventDate));
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
            return BreachDate == null ? (int?)null : ((DateTime)BreachDate - dateTime.AddDays(-CompletedEvent.Period.GetNumberOfPausedDays(CompletedEvent.EventDate, dateTime))).Days;
        }
    }
}
