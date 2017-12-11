using System;

namespace CPMS.Domain
{
    public class ClockStoppingCompletedEvent : CompletedEvent
    {
        private EventStatus _eventStatus;

        public override EventStatus EventStatus
        {
            get
            {
                _eventStatus = EventStatus.Closed;

                return _eventStatus;
            }
            set { _eventStatus = value; }
        }

        public override int GetDaysSpentAt(DateTime dateTime)
        {
            if (dateTime < EventDate)
            {
                throw new ArgumentException("Invalid DateTime parameter", "dateTime");
            }

            return 0;
        }

        public override void Validate()
        {
            if (!(Name.Code == EventCode.OutpatientDischarged || Name.Code == EventCode.InpatientDischarged || Name.Code == EventCode.PathwayCompletion || Name.Code == EventCode.PatientCancelEvent || Name.Code == EventCode.DidNotAttend))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Completed event {0}, on {1}, on pathway {2} should not stop the clock.", Name.Description, Period.Name, Period.Pathway.PPINumber),
                    Period = Period
                });
            }
        }
    }
}
