using System;

namespace CPMS.Domain
{
    public class ClockTickingCompletedEvent : CompletedEvent
    {
        private EventStatus _eventStatus;

        public override EventStatus EventStatus
        {
            get
            {
                _eventStatus = PostBreachDays == null ? EventStatus.Success : EventStatus.Breached;

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

            return (dateTime - EventDate).Days;
        }

        public override void Validate()
        {
            if (Name.Code == EventCode.PatientCancelEvent && Period.IsSecondCancelByPatient(Name.Code) ||
                Name.Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Name.Code) && Period.Pathway.Patient.IsChild(EventDate) ||
                Name.Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Name.Code) && Period.AbleToNotAttendFirstEvent ||
                Name.Code == EventCode.DidNotAttend && !(Period.Pathway.Patient.IsChild(EventDate) || Period.AbleToNotAttendFirstEvent))
            {
                 OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Completed event {0}, on {1}, on pathway {2} should not continue ticking the clock.", Name.Description, Period.Name, Period.Pathway.PPINumber),
                    Period = Period
                });
            }
        }
    }
}
