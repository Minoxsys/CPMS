using System;
using System.Collections.Generic;
using System.Linq;

namespace CPMS.Patient.Domain
{
    public abstract class Period : ValidationBase
    {
        private const int FirstDay = 1;
        private const int SecondTime = 2;

        protected Period()
        {
            Events = new List<Event>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Pathway Pathway { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? StopDate { get; set; }

        public IList<Event> Events { get; set; }

        public bool IsSecondCancelByPatient(EventCode code)
        {
            var patientCancels = Events.Count(e => e.Code == EventCode.PatientCancelEvent) + (code == EventCode.PatientCancelEvent ? 1 : 0);
            var hospitalCancels = Events.Count(e => e.Code == EventCode.HospitalCancelEvent) + (code == EventCode.HospitalCancelEvent ? 1 : 0);

            return patientCancels - hospitalCancels == SecondTime;
        }

        public bool IsSecondDidNotAttend(EventCode code)
        {
            return Events.Count(e => e.Code == EventCode.DidNotAttend) + (code == EventCode.DidNotAttend ? 1 : 0) == SecondTime;
        }

        public bool HasAdmittedPatient()
        {
            return Events.Any(e => e.Code == EventCode.PatientAddedToInpatientWaitingList);
        }

        public int GetDaysSpentAt(DateTime dateTime)
        {
            var sortedEventsInSelectedInterval =
                Events.Where(ev => ev.EventDate <= dateTime).OrderBy(ev => ev.EventDate).ToList();

            var total = CalculateNumberOfSpentDaysAt(sortedEventsInSelectedInterval, dateTime);

            return total;
        }

        public int GetNumberOfPausedDays(DateTime fromDateTime, DateTime toDateTime)
        {
            if (fromDateTime >= toDateTime) return 0;

            var sortedEventsInSelectedInterval =
                  Events.Where(ev => ev.EventDate >= fromDateTime && ev.EventDate <= toDateTime).OrderBy(ev => ev.EventDate).ToList();

            var spentDays = CalculateNumberOfSpentDaysAt(sortedEventsInSelectedInterval, toDateTime);
            var totalPastDays = (toDateTime - fromDateTime).Days + FirstDay;

            return totalPastDays - spentDays;
        }

        public int GetDelayOrAdvancementDays(DateTime dateTime)
        {
            var sortedEventsInSelectedInterval = Events.Where(ev => ev.EventDate <= dateTime).OrderBy(ev => ev.EventDate);
            var advancementDays = sortedEventsInSelectedInterval.Where(@event => @event.TargetDate != null).Sum(@event => (@event.TargetDate.Value - @event.EventDate).Days);

            return advancementDays;
        }

        public void Add(Event @event)
        {
            if (Events.Any(e => e.EventDate > @event.EventDate))
            {
                OnValidationFailed(new Error
               {
                   Message = string.Format("Event {0} on {1}, on pathway {2}, should not have the date {3} in the past.", @event.Code.GetDescription(), Name, Pathway.PPINumber, @event.EventDate),
                   Period = this
               });
            }

            Events.Add(@event);
        }

        public void MapPlannedEventToEvent(Event @event, PlannedEvent plannedEvent, EventCode targetEventCode)
        {
            if (!(Events.Any(p => p.Code == targetEventCode)))
            {
                OnValidationFailed(new Error
                {
                    Message = string.Format("Planned event {0} on {1}, on pathway {2}, does not have target reference event {3}.", plannedEvent.Code.GetDescription(), @event.Period.Name, @event.Period.Pathway.PPINumber, targetEventCode.GetDescription()),
                    Period = @event.Period
                });
            }

            plannedEvent.Event = @event;
        }

        public PeriodStatus GetPeriodStatus()
        {
            if (Events.OrderBy(ev => ev.EventDate).Last().EventStatus == EventStatus.Paused)
            {
                return PeriodStatus.Paused;
            }

            return StopDate != null ? PeriodStatus.Ended : PeriodStatus.InProgress;
        }

        private int CalculateNumberOfSpentDaysAt(IList<Event> sortedEventsInSelectedInterval, DateTime atDateTime)
        {
            int total = 0;
            foreach (var @event in sortedEventsInSelectedInterval)
            {
                if (@event != sortedEventsInSelectedInterval.Last())
                {
                    total +=
                        @event.GetDaysSpentAt(
                            sortedEventsInSelectedInterval[sortedEventsInSelectedInterval.IndexOf(@event) + 1].EventDate);
                }
                else
                {
                    total += @event.GetDaysSpentAt(atDateTime) + FirstDay;
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
