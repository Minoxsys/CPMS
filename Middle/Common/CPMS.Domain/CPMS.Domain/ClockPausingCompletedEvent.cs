using System;

namespace CPMS.Domain
{
    public class ClockPausingCompletedEvent : CompletedEvent
    {
        private EventStatus _eventStatus;

        public override EventStatus EventStatus
        {
            get
            {
                _eventStatus = EventStatus.Paused;
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
            if (!string.IsNullOrEmpty(Period.GetPauseValidationMessage))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Completed event {0}, on {1}, on pathway {2} {3}", Name.Description, Period.Name, Period.Pathway.PPINumber, Period.GetPauseValidationMessage),
                    Period = Period
                });
            }

            if (Name.Code == EventCode.PatientCancelEvent && Period.IsSecondCancelByPatient(Name.Code) ||
                Name.Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Name.Code) && Period.Pathway.Patient.IsChild(EventDate) ||
                Name.Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Name.Code) && Period.AbleToNotAttendFirstEvent ||
                Name.Code == EventCode.DidNotAttend && !(Period.Pathway.Patient.IsChild(EventDate) || Period.AbleToNotAttendFirstEvent) ||
                Name.Code == EventCode.HospitalCancelEvent || Name.Code != EventCode.InpatientTCIOffer)
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Completed event {0}, on {1}, on pathway {2} should not pause the clock.", Name.Description, Period.Name, Period.Pathway.PPINumber),
                    Period = Period
                });
            }
        }
    }
}
