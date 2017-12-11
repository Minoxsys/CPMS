using System;
using System.Collections.Generic;
using System.Linq;

namespace CPMS.Domain
{
    public abstract class Period : ValidationBase
    {
        private const int FirstDay = 1;
        private const int SecondTime = 2;

        protected Period()
        {
            CompletedEvents = new List<CompletedEvent>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Pathway Pathway { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? StopDate { get; set; }

        public IList<CompletedEvent> CompletedEvents { get; set; }

        public bool IsSecondCancelByPatient(EventCode code)
        {
            var patientCancels = CompletedEvents.Count(completedEvent => completedEvent.Name.Code == EventCode.PatientCancelEvent) + (code == EventCode.PatientCancelEvent ? 1 : 0);
            var hospitalCancels = CompletedEvents.Count(completedEvent => completedEvent.Name.Code == EventCode.HospitalCancelEvent) + (code == EventCode.HospitalCancelEvent ? 1 : 0);

            return patientCancels - hospitalCancels == SecondTime;
        }

        public bool IsSecondDidNotAttend(EventCode code)
        {
            return CompletedEvents.Count(completedEvent => completedEvent.Name.Code == EventCode.DidNotAttend) + (code == EventCode.DidNotAttend ? 1 : 0) == SecondTime;
        }

        public bool HasAdmittedPatient()
        {
            return CompletedEvents.Any(completedEvent => completedEvent.Name.Code == EventCode.PatientAddedToInpatientWaitingList);
        }

        public int GetDaysSpentAt(DateTime dateTime)
        {
            var sortedCompletedEventsInSelectedInterval = CompletedEvents
                .Where(completedEvent => completedEvent.EventDate <= dateTime)
                .OrderBy(completedEvent => completedEvent.EventDate)
                .ToList();

            var total = CalculateNumberOfSpentDaysAt(sortedCompletedEventsInSelectedInterval, dateTime);

            return total;
        }

        public int GetNumberOfPausedDays(DateTime fromDateTime, DateTime toDateTime)
        {
            if (fromDateTime >= toDateTime) return 0;

            var sortedCompletedEventsInSelectedInterval = CompletedEvents
                    .Where(completedEvent => completedEvent.EventDate >= fromDateTime && completedEvent.EventDate <= toDateTime)
                    .OrderBy(completedEvent => completedEvent.EventDate)
                    .ToList();

            var spentDays = CalculateNumberOfSpentDaysAt(sortedCompletedEventsInSelectedInterval, toDateTime);
            var totalPastDays = (toDateTime - fromDateTime).Days + FirstDay;

            return totalPastDays - spentDays;
        }

        public int GetDelayOrAdvancementDays(DateTime dateTime)
        {
            var sortedCompletedEventsInSelectedInterval = CompletedEvents.Where(completedEvent => completedEvent.EventDate <= dateTime).OrderBy(completedEvent => completedEvent.EventDate);
            var advancementDays = sortedCompletedEventsInSelectedInterval.Where(completedEvent => completedEvent.TargetDate != null).Sum(completedEvent => (completedEvent.TargetDate.Value - completedEvent.EventDate).Days);

            return advancementDays;
        }

        public void Add(CompletedEvent completedEvent)
        {
            if (CompletedEvents.Any(@event => @event.EventDate > completedEvent.EventDate))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Completed event {0} on {1}, on pathway {2}, should not have the date {3} in the past.", completedEvent.Name.Description, Name, Pathway.PPINumber, completedEvent.EventDate),
                    Period = this
                });
            }

            CompletedEvents.Add(completedEvent);
        }

        public void MapEventMilestoneToCompletedEvent(CompletedEvent completedEvent, EventMilestone eventMilestone, EventName targetEventName)
        {
            if (!(CompletedEvents.Any(@event => @event.Name.Code == targetEventName.Code)))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Event Milestone {0} on {1}, on pathway {2}, does not have target reference completed event {3}.", eventMilestone.Name.Description, completedEvent.Period.Name, completedEvent.Period.Pathway.PPINumber, targetEventName.Description),
                    Period = completedEvent.Period
                });
            }

            eventMilestone.CompletedEvent = completedEvent;
        }

        public PeriodStatus GetPeriodStatus()
        {
            if (CompletedEvents.OrderBy(completedEvent => completedEvent.EventDate).Last().EventStatus == EventStatus.Paused)
            {
                return PeriodStatus.Paused;
            }

            return StopDate != null ? PeriodStatus.Ended : PeriodStatus.InProgress;
        }

        private int CalculateNumberOfSpentDaysAt(IList<CompletedEvent> sortedCompletedEventsInSelectedInterval, DateTime atDateTime)
        {
            int total = 0;
            foreach (var completedEvent in sortedCompletedEventsInSelectedInterval)
            {
                if (completedEvent != sortedCompletedEventsInSelectedInterval.Last())
                {
                    total +=
                        completedEvent.GetDaysSpentAt(
                            sortedCompletedEventsInSelectedInterval[sortedCompletedEventsInSelectedInterval.IndexOf(completedEvent) + 1].EventDate);
                }
                else
                {
                    total += completedEvent.GetDaysSpentAt(atDateTime) + FirstDay;
                }
            }
            return total;
        }

        public virtual int MaximumNumberOfDays { get; set; }

        public virtual bool ShouldCountForBreaches { get; set; }

        public abstract string GetPauseValidationMessage { get; }

        public abstract bool AbleToNotAttendFirstEvent { get; }

        public abstract int GetDaysRemainingAt(DateTime dateTime);
    }
}
