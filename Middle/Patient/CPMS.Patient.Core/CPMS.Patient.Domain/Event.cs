using System;

namespace CPMS.Patient.Domain
{
    public abstract class Event : ValidationBase
    {
        private int? _postBreachDays;
        private EventBreachStatus _breachStatus;

        public int Id { get; set; }

        public EventCode Code { get; set; }

        public DateTime EventDate { get; set; }

        public string Comments { get; set; }

        public bool IsActive { get; set; }

        public bool Cancer { get; set; }

        public int? PostBreachDays
        {
            get
            {
                if (TargetDate == null || EventDate <= TargetDate)
                {
                    _postBreachDays = null;
                }
                else
                {
                    _postBreachDays = (EventDate - TargetDate.Value).Days;
                }
                return _postBreachDays;
            }
            set { _postBreachDays = value; }
        }

        public EventBreachStatus BreachStatus
        {
            get
            {
                _breachStatus = EventStatus == EventStatus.Breached
                    ? EventBreachStatus.Breached
                    : EventBreachStatus.Success;
                return _breachStatus;
            }
            set { _breachStatus = value; }
        }

        public Clinician Clinician { get; set; }

        public DateTime? TargetDate { get; set; }

        public Period Period { get; set; }

        public virtual EventStatus EventStatus { get; set; }

        public abstract int GetDaysSpentAt(DateTime dateTime);

        public abstract void Validate();
    }
}
