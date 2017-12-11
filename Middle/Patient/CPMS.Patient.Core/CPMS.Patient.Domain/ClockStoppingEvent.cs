using System;

namespace CPMS.Patient.Domain
{
    public class ClockStoppingEvent : Event
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
            //if (!(Code == EventCode.OutpatientDischarged || Code == EventCode.InpatientDischarged || Code == EventCode.PathwayCompletion || Code == EventCode.PatientCancelEvent || Code == EventCode.DidNotAttend))
            //{
            //    OnValidationFailed(new Error
            //    {
            //        Message = string.Format("Event {0}, on {1}, on pathway {2} should not stop the clock.", Code.GetDescription(), Period.Name, Period.Pathway.PPINumber),
            //        Period = Period
            //    });
            //}
        }
    }
}
