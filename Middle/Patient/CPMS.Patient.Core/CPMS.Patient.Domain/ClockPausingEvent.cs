using System;

namespace CPMS.Patient.Domain
{
    public class ClockPausingEvent : Event
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
                OnValidationFailed(new Error
                {
                    Message = string.Format("Event {0}, on {1}, on pathway {2} {3}", Code.GetDescription(), Period.Name, Period.Pathway.PPINumber, Period.GetPauseValidationMessage),
                    Period = Period
                });
            }

            //if (Code == EventCode.PatientCancelEvent && Period.IsSecondCancelByPatient(Code) ||
            //    Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Code) && Period.Pathway.Patient.IsChild(EventDate) ||
            //    Code == EventCode.DidNotAttend && Period.IsSecondDidNotAttend(Code) && Period.AbleToNotAttendFirstEvent ||
            //    Code == EventCode.DidNotAttend && !(Period.Pathway.Patient.IsChild(EventDate) || Period.AbleToNotAttendFirstEvent) ||
            //    Code == EventCode.HospitalCancelEvent || Code != EventCode.InpatientTCIOffer)
            //{
            //    OnValidationFailed(new Error
            //    {
            //        Message = string.Format("Event {0}, on {1}, on pathway {2} should not pause the clock.", Code.GetDescription(), Period.Name, Period.Pathway.PPINumber),
            //        Period = Period
            //    });
            //}
        }
    }
}
