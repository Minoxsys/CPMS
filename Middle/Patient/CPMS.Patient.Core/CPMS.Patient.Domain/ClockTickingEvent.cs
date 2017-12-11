using System;

namespace CPMS.Patient.Domain
{
    public class ClockTickingEvent : Event
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
            if (Code == EventCode.PatientCancelEvent && Period.IsSecondCancelByPatient(Code) ||
                Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Code) && Period.Pathway.Patient.IsChild(EventDate) ||
                Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Code) && Period.AbleToNotAttendFirstEvent ||
                Code == EventCode.DidNotAttend && !(Period.Pathway.Patient.IsChild(EventDate) || Period.AbleToNotAttendFirstEvent))
            {
                 OnValidationFailed(new Error
                {
                    Message = string.Format("Event {0}, on {1}, on pathway {2} should not continue ticking the clock.", Code.GetDescription(), Period.Name, Period.Pathway.PPINumber),
                    Period = Period
                });
            }
        }
    }
}
